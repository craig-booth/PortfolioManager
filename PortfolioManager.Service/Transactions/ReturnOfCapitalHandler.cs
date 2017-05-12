using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Utils;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Transactions
{
    class ReturnOfCapitalHandler : TransacactionHandler, ITransactionHandler
    {

        public ReturnOfCapitalHandler(IPortfolioQuery portfolioQuery, StockService stockService)
            : base (portfolioQuery, stockService)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var returnOfCapital = transaction as ReturnOfCapital;

            var stock = _StockService.Get(returnOfCapital.ASXCode, returnOfCapital.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(returnOfCapital, "Cannot have a return of capital for stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the transaction applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, returnOfCapital.RecordDate, returnOfCapital.RecordDate);

            if (parcels.Count == 0)
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
