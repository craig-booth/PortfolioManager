using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class UnitCountAdjustmnetHandler : TransacactionHandler, ITransactionHandler
    {

        public readonly ParcelService _ParcelService;
        public readonly StockService _StockService;

        public UnitCountAdjustmnetHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var unitCostAdjustment = transaction as UnitCountAdjustment;

            var stock = _StockService.Get(unitCostAdjustment.ASXCode, unitCostAdjustment.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(unitCostAdjustment, "Cannot adjust unit count of stapled securities. Adjust unit count of child securities instead");

            /* locate parcels that the split/consolidation applies to */
            var parcels = _ParcelService.GetParcels(stock, unitCostAdjustment.TransactionDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(unitCostAdjustment, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            foreach (ShareParcel parcel in parcels)
            {
                var newUnitCount = (int)Math.Round(parcel.Units * ((decimal)unitCostAdjustment.NewUnits / (decimal)unitCostAdjustment.OriginalUnits));
                ModifyParcel(unitOfWork, parcel, unitCostAdjustment.TransactionDate, ParcelEvent.UnitCountChange, newUnitCount, parcel.CostBase, "");
            }

        }


    }
}
