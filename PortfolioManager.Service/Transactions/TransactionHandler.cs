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

        public TransacactionHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        protected void AddParcel(IPortfolioUnitOfWork unitOfWork, DateTime aquisitionDate, Stock stock, int units, decimal unitPrice, decimal amount, decimal costBase, ParcelEvent parcelEvent)
        {
            /* Handle Stapled Securities */
            if (stock.Type == StockType.StapledSecurity)
            {
                /* Get child stocks */
                var childStocks = _StockService.GetChildStocks(stock, aquisitionDate);

                /* Apportion amount and cost base */
                ApportionedCurrencyValue[] apportionedAmounts = new ApportionedCurrencyValue[childStocks.Count];
                ApportionedCurrencyValue[] apportionedCostBases = new ApportionedCurrencyValue[childStocks.Count];
                ApportionedCurrencyValue[] apportionedUnitPrices = new ApportionedCurrencyValue[childStocks.Count];
                int i = 0;
                foreach (Stock childStock in childStocks)
                {
                    decimal percentageOfParent = _StockService.PercentageOfParentCostBase(childStock, aquisitionDate);
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
                var purchaseId = Guid.NewGuid();
                foreach (Stock childStock in childStocks)
                {
                    var childParcel = new ShareParcel(aquisitionDate, childStock.Id, units, apportionedUnitPrices[i].Amount, apportionedAmounts[i].Amount, apportionedCostBases[i].Amount, purchaseId, parcelEvent);

                    unitOfWork.ParcelRepository.Add(childParcel);

                    i++;
                }
            }
            else
            {
                var parcel = new ShareParcel(aquisitionDate, stock.Id, units, unitPrice, amount, costBase, parcelEvent);
                unitOfWork.ParcelRepository.Add(parcel);
            }
        }

        protected void ModifyParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime changeDate, ParcelEvent parcelEvent, Action<ShareParcel> change)
        {
            // Check that this is the latest version of this parcel
            if (parcel.ToDate != DateTimeConstants.NoEndDate)
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

        protected void DisposeOfParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime disposalDate, int units, decimal amountReceived, string description)
        {
            /* Modify Parcel */
            var costBase = parcel.CostBase * ((decimal)units / parcel.Units);
            var amount = parcel.Amount * ((decimal)units / parcel.Units);
            ModifyParcel(unitOfWork, parcel, disposalDate, ParcelEvent.Disposal, x => { x.Units -= units; x.CostBase -= costBase; x.Amount -= amount; });
        }

        protected void AddCGTEvent(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime eventDate, decimal amount)
        {
            unitOfWork.CGTEventRepository.Add(PortfolioUtils.CreateCGTEvent(parcel, eventDate, amount));
        }

        protected void AddCGTEvent(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime eventDate, int units, decimal amount)
        {
            unitOfWork.CGTEventRepository.Add(PortfolioUtils.CreateCGTEvent(parcel, eventDate, units, amount));
        }

        protected void CashAccountTransaction(IPortfolioUnitOfWork unitOfWork, CashAccountTransactionType type, DateTime date, string description, decimal amount)
        {
            var cashTransaction = new CashAccountTransaction()
            {
                Type = type,
                Date = date,
                Description = description,
                Amount = amount
            };
            unitOfWork.CashTransactionRepository.Add(cashTransaction);
        }
 
    }

}
