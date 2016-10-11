using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    interface ITransactionHandler
    {
        void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction);
    }

    public abstract class TransacactionHandler
    {

        protected readonly ParcelService _ParcelService;
        protected readonly StockService _StockService;

        protected TransacactionHandler()
        {

        }

        public TransacactionHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        protected void AddParcel(IPortfolioUnitOfWork unitOfWork, DateTime aquisitionDate, DateTime fromDate, Stock stock, int units, decimal unitPrice, decimal amount, decimal costBase, Guid purchaseId, ParcelEvent parcelEvent)
        {
            /* Handle Stapled Securities */
            if (stock.Type == StockType.StapledSecurity)
            {
                /* Get child stocks */
                var childStocks = _StockService.GetChildStocks(stock, fromDate);

                /* Apportion amount and cost base */
                ApportionedCurrencyValue[] apportionedAmounts = new ApportionedCurrencyValue[childStocks.Count];
                ApportionedCurrencyValue[] apportionedCostBases = new ApportionedCurrencyValue[childStocks.Count];
                ApportionedCurrencyValue[] apportionedUnitPrices = new ApportionedCurrencyValue[childStocks.Count];
                int i = 0;
                foreach (Stock childStock in childStocks)
                {
                    decimal percentageOfParent = _StockService.PercentageOfParentCostBase(childStock, fromDate);
                    int relativeValue = (int)(percentageOfParent * 10000);

                    apportionedAmounts[i].Units = relativeValue;
                    apportionedCostBases[i].Units = relativeValue;
                    apportionedUnitPrices[i].Units = relativeValue;
                    i++;
                }
                MathUtils.ApportionAmount(amount, apportionedAmounts);
                MathUtils.ApportionAmount(costBase, apportionedCostBases);
                MathUtils.ApportionAmount(unitPrice, apportionedUnitPrices);

                i = 0;
                foreach (Stock childStock in childStocks)
                {
                    var childParcel = new ShareParcel(fromDate, DateUtils.NoEndDate, aquisitionDate, childStock.Id, units, apportionedUnitPrices[i].Amount, apportionedAmounts[i].Amount, apportionedCostBases[i].Amount, purchaseId, parcelEvent);

                    unitOfWork.ParcelRepository.Add(childParcel);

                    i++;
                }
            }
            else
            {
                var parcel = new ShareParcel(fromDate, DateUtils.NoEndDate, aquisitionDate, stock.Id, units, unitPrice, amount, costBase, purchaseId, parcelEvent);
                unitOfWork.ParcelRepository.Add(parcel);
            }
        }

        protected void ModifyParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime changeDate, ParcelEvent parcelEvent, Action<ShareParcel> change)
        {
            // Check that this is the latest version of this parcel
            if (parcel.ToDate != DateUtils.NoEndDate)
                throw new AttemptToModifyPreviousParcelVersion(parcel.Id, "");

            if (parcel.FromDate == changeDate)
            {
                parcel.Event = parcelEvent;
                change(parcel);
                unitOfWork.ParcelRepository.Update(parcel);
            }
            else
            {
                /* Create new effective dated entity */
                var newParcel = parcel.CreateNewEffectiveEntity(changeDate);
                newParcel.Event = parcelEvent;
                change(newParcel);

                /* Add new record */
                if (newParcel.Units > 0)
                    unitOfWork.ParcelRepository.Add(newParcel);

                /* End existing effective dated entity */
                parcel.EndEntity(changeDate.AddDays(-1));
                unitOfWork.ParcelRepository.Update(parcel);
            }

        }

        protected void DisposeOfParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime disposalDate, int unitsSold, decimal amountReceived)
        {
            /* Modify Parcel */
            var costBase = parcel.CostBase * ((decimal)unitsSold / parcel.Units);
            var amount = parcel.Amount * ((decimal)unitsSold / parcel.Units);
            ModifyParcel(unitOfWork, parcel, disposalDate, ParcelEvent.Disposal, x => { x.Units -= unitsSold; x.CostBase -= costBase; x.Amount -= amount; });

            var cgtEvent = new CGTEvent(parcel.Stock, disposalDate, unitsSold, costBase, amountReceived, amountReceived - costBase, CGTCalculator.CGTMethodForParcel(parcel, disposalDate));
            unitOfWork.CGTEventRepository.Add(cgtEvent);
        }

        protected void ReduceParcelCostBase(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime date, decimal amount)
        {
            if (amount <= parcel.CostBase)
                ModifyParcel(unitOfWork, parcel, date, ParcelEvent.CostBaseReduction, x => { x.CostBase -= amount; });
            else
            {
                ModifyParcel(unitOfWork, parcel, date, ParcelEvent.CostBaseReduction, x => { x.CostBase = 0.00m; });

                var cgtEvent = new CGTEvent(parcel.Stock, date, parcel.Units, parcel.CostBase, amount - parcel.CostBase, amount - parcel.CostBase, CGTMethod.Other);
                unitOfWork.CGTEventRepository.Add(cgtEvent);
            }
        }

        protected void CashAccountTransaction(IPortfolioUnitOfWork unitOfWork, CashAccountTransactionType type, DateTime date, string description, decimal amount)
        {
            if (amount != 0.00m)
            {
                var cashTransaction = new CashAccountTransaction()
                {
                    Type = type,
                    Date = date,
                    Description = description,
                    Amount = amount
                };
                unitOfWork.CashAccountRepository.Add(cashTransaction);
            }
        }
 
    }

}
