using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    class ReturnOfCapitalHandler : TransacactionHandler, ITransactionHandler
    {

        public ReturnOfCapitalHandler(ParcelService parcelService, StockService stockService)
            : base (parcelService, stockService)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var returnOfCapital = transaction as ReturnOfCapital;

            var stock = _StockService.Get(returnOfCapital.ASXCode, returnOfCapital.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(returnOfCapital, "Cannot have a return of capital for stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the transaction applies to */
            var parcels = _ParcelService.GetParcels(stock, returnOfCapital.RecordDate);

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
                CashAccountTransaction(unitOfWork, CashAccountTransactionType.Transfer, returnOfCapital.TransactionDate, String.Format("Return of capital for {0}", returnOfCapital.ASXCode), totalAmount);                
        }
    }
}
