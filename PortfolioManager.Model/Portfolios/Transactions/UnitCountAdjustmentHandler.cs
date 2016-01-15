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
    class UnitCountAdjustmentHandler : TransacactionHandler, ITransactionHandler
    {

        public readonly ParcelService _ParcelService;
        public readonly StockService _StockService;

        public UnitCountAdjustmentHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var unitCountAdjustment = transaction as UnitCountAdjustment;

            var stock = _StockService.Get(unitCountAdjustment.ASXCode, unitCountAdjustment.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(unitCountAdjustment, "Cannot adjust unit count of stapled securities. Adjust unit count of child securities instead");

            /* locate parcels that the split/consolidation applies to */
            var parcels = _ParcelService.GetParcels(stock, unitCountAdjustment.TransactionDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(unitCountAdjustment, "No parcels found for transaction");

            /* Determine total number of units after split/consolidation */
            var originalTotalUnitCount = parcels.Sum(x => x.Units);
            var newTotalUnitCount = (int)Math.Round(originalTotalUnitCount * ((decimal)unitCountAdjustment.NewUnits / (decimal)unitCountAdjustment.OriginalUnits));

            /* Apportion unit count over parcels */
            var newUnitCounts = PortfolioUtils.ApportionAmountOverParcels(parcels, newTotalUnitCount);

            /* Reduce cost base of parcels */
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                ModifyParcel(unitOfWork, parcel, unitCountAdjustment.TransactionDate, ParcelEvent.UnitCountChange, newUnitCounts[i++].Amount, parcel.CostBase, "");

        }


    }
}
