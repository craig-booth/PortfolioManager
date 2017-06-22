using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Utils;


namespace PortfolioManager.Service.Transactions
{
    class UnitCountAdjustmentHandler : TransacactionHandler, ITransactionHandler
    {
        public UnitCountAdjustmentHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
            : base (portfolioQuery, stockQuery)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var unitCountAdjustment = transaction as UnitCountAdjustment;

            var stock = _StockQuery.GetByASXCode(unitCountAdjustment.ASXCode, unitCountAdjustment.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(unitCountAdjustment, "Cannot adjust unit count of stapled securities. Adjust unit count of child securities instead");

            /* locate parcels that the split/consolidation applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, unitCountAdjustment.TransactionDate, unitCountAdjustment.TransactionDate);

            if (!parcels.Any())
                throw new NoParcelsForTransaction(unitCountAdjustment, "No parcels found for transaction");

            /* Determine total number of units after split/consolidation */
            var originalTotalUnitCount = parcels.Sum(x => x.Units);
            var newTotalUnitCount = (int)Math.Ceiling(originalTotalUnitCount * ((decimal)unitCountAdjustment.NewUnits / (decimal)unitCountAdjustment.OriginalUnits));

            /* Apportion unit count over parcels */
            var newUnitCounts = PortfolioUtils.ApportionAmountOverParcels(parcels, newTotalUnitCount);

            /* Change the unit count of the parcels */
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                ChangeParcelUnitCount(unitOfWork, parcel, unitCountAdjustment.TransactionDate, newUnitCounts[i++].Amount, transaction.Id);
        }

    }
}
