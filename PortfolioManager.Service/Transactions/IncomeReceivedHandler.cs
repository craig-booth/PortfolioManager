using System;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Transactions
{
    class IncomeReceivedHandler : TransacactionHandler, ITransactionHandler
    {

        public IncomeReceivedHandler(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
            : base (portfolioQuery, stockExchange)
        {
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var incomeReceived = transaction as IncomeReceived;

            var stock = _StockExchange.Stocks.Get(incomeReceived.ASXCode, incomeReceived.RecordDate);

            if (stock is StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(incomeReceived, "Cannot have a income for stapled securities. Income should be recorded against child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, incomeReceived.RecordDate, incomeReceived.RecordDate);

            if (!parcels.Any())
                throw new NoParcelsForTransaction(incomeReceived, "No parcels found for transaction");

            /* Handle any tax deferred amount recieved */
            if (incomeReceived.TaxDeferred > 0)
            {
                /* Apportion amount between parcels */
                ApportionedCurrencyValue[] apportionedAmounts = new ApportionedCurrencyValue[parcels.Count()];
                int i = 0;
                foreach (ShareParcel parcel in parcels)
                    apportionedAmounts[i++].Units = parcel.Units;
                MathUtils.ApportionAmount(incomeReceived.TaxDeferred, apportionedAmounts);

                /* Reduce cost base of parcels */
                i = 0;
                foreach (ShareParcel parcel in parcels)
                {
                    decimal costBaseReduction = apportionedAmounts[i++].Amount;

                    ReduceParcelCostBase(unitOfWork, parcel, incomeReceived.RecordDate, costBaseReduction, transaction.Id);
                }
            }

            if (incomeReceived.CreateCashTransaction)
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Transfer, incomeReceived.TransactionDate, String.Format("Distribution for {0}", incomeReceived.ASXCode), incomeReceived.CashIncome);

            UpdateDRPCashBalance(unitOfWork, stock, incomeReceived.TransactionDate, incomeReceived.DRPCashBalance);
        }
    }
}
