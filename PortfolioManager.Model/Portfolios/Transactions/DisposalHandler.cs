using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    class DisposalHandler : ITransactionHandler
    {
        public readonly ParcelService _ParcelService;
        public readonly StockService _StockService;

        public DisposalHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }
        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var disposal = transaction as Disposal;

            var stock = _StockService.Get(disposal.ASXCode, disposal.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(disposal, "Cannot dispose of child securities. Dispose of stapled security instead");

            /* Create CGT calculator */
            var CGTCalculator = new CGTCalculator();

            /* Determine which parcels to sell based on CGT method */
            IReadOnlyCollection<ShareParcel> ownedParcels;
            if (stock.Type == StockType.StapledSecurity)
                ownedParcels = _ParcelService.GetStapledSecurityParcels(stock, disposal.TransactionDate);
            else
                ownedParcels = _ParcelService.GetParcels(stock, disposal.TransactionDate);
            decimal amountReceived = (disposal.Units * disposal.AveragePrice) - disposal.TransactionCosts;
            var CGTCalculation = CGTCalculator.CalculateCapitalGain(ownedParcels, disposal.TransactionDate, disposal.Units, amountReceived, disposal.CGTMethod);

            if (CGTCalculation.UnitsSold == 0)
                throw new NoParcelsForTransaction(disposal, "No parcels found for transaction");
            else if (CGTCalculation.UnitsSold < disposal.Units)
                throw new NotEnoughSharesForDisposal(disposal, "Not enough shares for disposal");
            
            /* dispose of select parcels */
            if (stock.Type == StockType.StapledSecurity)
            {
                foreach (ParcelSold parcelSold in CGTCalculation.ParcelsSold)
                {
                    var childStocks = _StockService.GetChildStocks(stock, disposal.TransactionDate);

                    // Apportion amount based on NTA of child stocks
                    var amountsReceived = PortfolioUtils.ApportionAmountOverChildStocks(childStocks, disposal.TransactionDate, parcelSold.AmountReceived);

                    int i = 0;
                    foreach (var childStock in childStocks)
                    {
                        var childParcels = _ParcelService.GetParcels(childStock, disposal.TransactionDate);

                        var childParcel = childParcels.First(x => x.PurchaseId == parcelSold.Parcel.PurchaseId);
                        _ParcelService.DisposeOfParcel(unitOfWork, childParcel, disposal.TransactionDate, parcelSold.UnitsSold, amountsReceived[i].Amount, disposal.Description);

                        i++;
                    }

                };
            }
            else
            {
                foreach (ParcelSold parcelSold in CGTCalculation.ParcelsSold)
                    _ParcelService.DisposeOfParcel(unitOfWork, parcelSold.Parcel, disposal.TransactionDate, parcelSold.UnitsSold, parcelSold.AmountReceived, disposal.Description);
            }

            //CashAccount.AddTransaction(CashAccountTransactionType.Transfer, disposal.TransactionDate, String.Format("Sale of {0}", disposal.ASXCode), amountReceived + disposal.TransactionCosts);

        }
    }
}
