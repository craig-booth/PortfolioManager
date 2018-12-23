using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.Transactions
{
    public class DisposalHandler : ITransactionHandler
    { 
        private HoldingCollection _Holdings;
        private CashAccount _CashAccount;

        public DisposalHandler(HoldingCollection holdings, CashAccount cashAccount)
        {
            _Holdings = holdings;
            _CashAccount = cashAccount;
        }

        public void ApplyTransaction(Transaction transaction)
        {
            var disposal = transaction as Disposal;

            var holding = _Holdings.Get(disposal.Stock.Id);

            // Determine which parcels to sell based on CGT method 
            decimal amountReceived = (disposal.Units * disposal.AveragePrice) - disposal.TransactionCosts;
            var cgtCalculation = CGTCalculator.CalculateCapitalGain(holding.Parcels(disposal.TransactionDate), disposal.TransactionDate, disposal.Units, amountReceived, disposal.CGTMethod);

            if (cgtCalculation.UnitsSold == 0)
                throw new NoParcelsForTransaction(disposal, "No parcels found for transaction");
            else if (cgtCalculation.UnitsSold < disposal.Units)
                throw new NotEnoughSharesForDisposal(disposal, "Not enough shares for disposal");
                   
            // Dispose of select parcels 
            if (disposal.Stock is StapledSecurity)
            {
             /*     foreach (ParcelSold parcelSold in cgtCalculation.ParcelsSold)
                  {
                      var childStocks = _StockQuery.GetChildStocks(stock.Id, disposal.TransactionDate);

                      // Apportion amount based on NTA of child stocks
                      var amountsReceived = PortfolioUtils.ApportionAmountOverChildStocks(childStocks, disposal.TransactionDate, parcelSold.AmountReceived, _StockQuery);

                      int i = 0;
                      foreach (var childStock in childStocks)
                      {
                          var childParcels = _PortfolioQuery.GetParcelsForStock(childStock.Id, disposal.TransactionDate, disposal.TransactionDate);

                          var childParcel = childParcels.First(x => x.PurchaseId == parcelSold.Parcel.PurchaseId);
                          DisposeOfParcel(unitOfWork, childParcel, disposal.TransactionDate, parcelSold.UnitsSold, amountsReceived[i].Amount, transaction.Id);

                          i++;
                      }

                  };  */
            } 
            else
            {
                foreach (ParcelSold parcelSold in cgtCalculation.ParcelsSold)
                    holding.DisposeOfParcel(parcelSold.Id, disposal.TransactionDate, parcelSold.UnitsSold, parcelSold.AmountReceived);
            } 
            
            if (disposal.CreateCashTransaction)
            {
                var cost = disposal.Units * disposal.AveragePrice;

                var asxCode = disposal.Stock.Properties[disposal.TransactionDate].ASXCode;
                _CashAccount.Transfer(disposal.TransactionDate, -cost, String.Format("Sale of {0}", asxCode));

                if (disposal.TransactionCosts > 0.00m)
                    _CashAccount.FeeDeducted(disposal.TransactionDate, disposal.TransactionCosts, String.Format("Brokerage for sale of {0}", asxCode));
            }
        }
    }
}
