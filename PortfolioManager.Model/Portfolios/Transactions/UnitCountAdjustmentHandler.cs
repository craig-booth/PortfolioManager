using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

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

            /* Reduce cost base of parcels */
            foreach (ShareParcel parcel in parcels)
            {
                var newUnitCount = (int)Math.Round(parcel.Units * ((decimal)unitCountAdjustment.NewUnits / (decimal)unitCountAdjustment.OriginalUnits));
                ModifyParcel(unitOfWork, parcel, unitCountAdjustment.TransactionDate, ParcelEvent.UnitCountChange, newUnitCount, parcel.CostBase, "");
            }

        }


    }
}
