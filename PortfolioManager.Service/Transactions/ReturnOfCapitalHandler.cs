using System;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Transactions
{
    class ReturnOfCapitalHandler : TransacactionHandler, ITransactionHandler
    {

        public ReturnOfCapitalHandler(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
            : base (portfolioQuery, stockExchange)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var returnOfCapital = transaction as ReturnOfCapital;

            var stock = _StockExchange.Stocks.Get(returnOfCapital.ASXCode, returnOfCapital.RecordDate);

            if (stock is StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(returnOfCapital, "Cannot have a return of capital for stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the transaction applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, returnOfCapital.RecordDate, returnOfCapital.RecordDate);

            if (!parcels.Any())
                throw new NoParcelsForTransaction(returnOfCapital, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            decimal totalAmount = 0;
            foreach (ShareParcel parcel in parcels)
            {
                var costBaseReduction = parcel.Units * returnOfCapital.Amount;

                ReduceParcelCostBase(unitOfWork, parcel, returnOfCapital.RecordDate, costBaseReduction, transaction.Id);

                totalAmount += costBaseReduction;
            }

            if (returnOfCapital.CreateCashTransaction)
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Transfer, returnOfCapital.TransactionDate, String.Format("Return of capital for {0}", returnOfCapital.ASXCode), totalAmount);                
        }
    }
}
