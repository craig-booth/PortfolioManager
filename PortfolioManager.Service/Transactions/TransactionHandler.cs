using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Transactions
{
    interface ITransactionHandler
    {
        void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction);
    }

    abstract class TransacactionHandler
    {
        protected readonly IPortfolioQuery _PortfolioQuery;
        protected readonly StockService _StockService;
        protected readonly PortfolioUtils _PortfolioUtils;

        protected TransacactionHandler()
        {

        }

        public TransacactionHandler(IPortfolioQuery portfolioQuery, StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
            _PortfolioUtils = new Utils.PortfolioUtils(_PortfolioQuery, _StockService); 
        }

        protected void AddParcel(IPortfolioUnitOfWork unitOfWork, DateTime aquisitionDate, DateTime fromDate, Stock stock, int units, decimal unitPrice, decimal amount, decimal costBase, Guid transactionId, Guid purchaseId)
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
                    var childParcel = new ShareParcel(fromDate, DateUtils.NoEndDate, aquisitionDate, childStock.Id, units, apportionedUnitPrices[i].Amount, apportionedAmounts[i].Amount, apportionedCostBases[i].Amount, purchaseId);
                    unitOfWork.ParcelRepository.Add(childParcel);

                    var parcelAudit = new ShareParcelAudit(childParcel.Id, aquisitionDate, transactionId, childParcel.Units, childParcel.CostBase, childParcel.Amount);
                    unitOfWork.ParcelAuditRepository.Add(parcelAudit);

                    i++;
                }
            }
            else
            {
                var parcel = new ShareParcel(fromDate, DateUtils.NoEndDate, aquisitionDate, stock.Id, units, unitPrice, amount, costBase, purchaseId);
                unitOfWork.ParcelRepository.Add(parcel);

                var parcelAudit = new ShareParcelAudit(parcel.Id, aquisitionDate, transactionId, parcel.Units, parcel.CostBase, parcel.Amount);
                unitOfWork.ParcelAuditRepository.Add(parcelAudit);
            }
        }

        private ShareParcel ModifyParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime changeDate, int newUnitCount, decimal costBaseChange, decimal amountChange, Guid transactionId)
        {
            // Check that this is the latest version of this parcel
            if (parcel.ToDate != DateUtils.NoEndDate)
                throw new AttemptToModifyPreviousParcelVersion(parcel.Id, "");

            if (parcel.FromDate == changeDate)
            {
                parcel.Units = newUnitCount;
                parcel.CostBase += costBaseChange;
                parcel.Amount += amountChange;

                unitOfWork.ParcelRepository.Update(parcel);

                var parcelAudit = new ShareParcelAudit(parcel.Id, changeDate, transactionId, newUnitCount, costBaseChange, amountChange);
                unitOfWork.ParcelAuditRepository.Add(parcelAudit);

                return parcel;
            }
            else
            {
                /* Create new effective dated entity */
                var newParcel = parcel.CreateNewEffectiveEntity(changeDate);

                newParcel.Units = newUnitCount;
                newParcel.CostBase += costBaseChange;
                newParcel.Amount += amountChange;

                /* Add new record */
                unitOfWork.ParcelRepository.Add(newParcel);

                /* End existing effective dated entity */
                parcel.EndEntity(changeDate.AddDays(-1));
                unitOfWork.ParcelRepository.Update(parcel);

                var parcelAudit = new ShareParcelAudit(parcel.Id, changeDate, transactionId, newUnitCount, costBaseChange, amountChange);
                unitOfWork.ParcelAuditRepository.Add(parcelAudit);

                return newParcel;
            }
        }

        protected void DisposeOfParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime disposalDate, int unitsSold, decimal amountReceived, Guid transactionId)
        {
            decimal costBase;
            decimal amount;

            if (unitsSold == parcel.Units)
            {
                costBase = parcel.CostBase;
                amount = parcel.Amount;

                var newParcel = ModifyParcel(unitOfWork, parcel, disposalDate, 0, -parcel.CostBase, -parcel.Amount, transactionId);
                EndParcel(unitOfWork, newParcel, disposalDate);
            }
            else
            {
                costBase = (parcel.CostBase * ((decimal)unitsSold / parcel.Units)).ToCurrency(RoundingRule.Round);
                amount = (parcel.Amount * ((decimal)unitsSold / parcel.Units)).ToCurrency(RoundingRule.Round);

                ModifyParcel(unitOfWork, parcel, disposalDate, parcel.Units - unitsSold, -costBase, -amount, transactionId);
            }


            var cgtEvent = new CGTEvent(parcel.Stock, disposalDate, unitsSold, costBase, amountReceived, amountReceived - costBase, CGTCalculator.CGTMethodForParcel(parcel, disposalDate));
            unitOfWork.CGTEventRepository.Add(cgtEvent);
        }

        protected void ReduceParcelCostBase(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime date, decimal amount, Guid transactionId)
        {
            if (amount <= parcel.CostBase)
                ModifyParcel(unitOfWork, parcel, date, parcel.Units, -amount, 0.00m, transactionId);
            else
            {
                ModifyParcel(unitOfWork, parcel, date, parcel.Units, -parcel.CostBase, 0.00m, transactionId);

                var cgtEvent = new CGTEvent(parcel.Stock, date, parcel.Units, parcel.CostBase, amount - parcel.CostBase, amount - parcel.CostBase, CGTMethod.Other);
                unitOfWork.CGTEventRepository.Add(cgtEvent);
            }
        }

        protected void ChangeParcelUnitCount(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime date, int newUnitCount, Guid transactionId)
        {
            ModifyParcel(unitOfWork, parcel, date, newUnitCount, 0.00m, 0.00m, transactionId);
        }

        protected void EndParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime date)
        {
            parcel.EndEntity(date);
            unitOfWork.ParcelRepository.Update(parcel);
        }

        protected void CashAccountTransaction(IPortfolioUnitOfWork unitOfWork, BankAccountTransactionType type, DateTime date, string description, decimal amount)
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

        protected void UpdateDRPCashBalance(IPortfolioUnitOfWork unitOfWork, Stock stock, DateTime balanceDate, decimal balance)
        {
            var drpCashBalance = unitOfWork.DRPCashBalanceRepository.Get(stock.Id, balanceDate);

            if (drpCashBalance == null)
            {
                drpCashBalance = new DRPCashBalance(stock.Id, balanceDate, DateUtils.NoEndDate, balance);

                unitOfWork.DRPCashBalanceRepository.Add(drpCashBalance);

            }
            else if (drpCashBalance.FromDate == balanceDate)
            {
                drpCashBalance.Balance = balance;

                unitOfWork.DRPCashBalanceRepository.Update(drpCashBalance);
            }
            else
            {
                /* Create new effective dated entity */
                var newDrpCashBalance = drpCashBalance.CreateNewEffectiveEntity(balanceDate);

                newDrpCashBalance.Balance = balance;

                /* Add new record */
                unitOfWork.DRPCashBalanceRepository.Add(newDrpCashBalance);

                /* End existing effective dated entity */
                drpCashBalance.EndEntity(balanceDate.AddDays(-1));
                unitOfWork.DRPCashBalanceRepository.Update(drpCashBalance);
            }
        }

    }

}
