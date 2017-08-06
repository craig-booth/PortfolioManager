using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Transactions
{
    class DisposalHandler : TransacactionHandler, ITransactionHandler
    {
        public DisposalHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
            : base (portfolioQuery, stockQuery)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var disposal = transaction as Disposal;

            var stock = _StockQuery.GetByASXCode(disposal.ASXCode, disposal.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(disposal, "Cannot dispose of child securities. Dispose of stapled security instead");

            /* Determine which parcels to sell based on CGT method */
            IEnumerable<ShareParcel> ownedParcels;
            if (stock.Type == StockType.StapledSecurity)
                ownedParcels = PortfolioUtils.GetStapledSecurityParcels(stock, disposal.TransactionDate, _PortfolioQuery, _StockQuery);
            else
                ownedParcels = _PortfolioQuery.GetParcelsForStock(stock.Id, disposal.TransactionDate, disposal.TransactionDate);
            decimal amountReceived = (disposal.Units * disposal.AveragePrice) - disposal.TransactionCosts;
            var cgtCalculation = CGTCalculator.CalculateCapitalGain(ownedParcels, disposal.TransactionDate, disposal.Units, amountReceived, disposal.CGTMethod);

            if (cgtCalculation.UnitsSold == 0)
                throw new NoParcelsForTransaction(disposal, "No parcels found for transaction");
            else if (cgtCalculation.UnitsSold < disposal.Units)
                throw new NotEnoughSharesForDisposal(disposal, "Not enough shares for disposal");
            
            /* dispose of select parcels */
            if (stock.Type == StockType.StapledSecurity)
            {
                foreach (ParcelSold parcelSold in cgtCalculation.ParcelsSold)
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

                };
            }
            else
            {
                foreach (ParcelSold parcelSold in cgtCalculation.ParcelsSold)
                    DisposeOfParcel(unitOfWork, parcelSold.Parcel, disposal.TransactionDate, parcelSold.UnitsSold, parcelSold.AmountReceived, transaction.Id);
            }

            if (disposal.CreateCashTransaction)
            {
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Transfer, disposal.TransactionDate, String.Format("Sale of {0}", disposal.ASXCode), disposal.Units * disposal.AveragePrice);
                CashAccountTransaction(unitOfWork, BankAccountTransactionType.Fee, disposal.TransactionDate, String.Format("Brokerage for sale of {0}", disposal.ASXCode), -1 * disposal.TransactionCosts);
            }
        }
    }
}
