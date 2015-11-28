using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class SplitConsolidationHandler : TransacactionHandler, ITransactionHandler
    {

        public readonly ParcelService _ParcelService;
        public readonly StockService _StockService;

        public SplitConsolidationHandler(ParcelService parcelService, StockService stockService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var splitConsolidation = transaction as SplitConsolidation;

            var stock = _StockService.Get(splitConsolidation.ASXCode, splitConsolidation.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(splitConsolidation, "Cannot split/consolidate stapled securities. Split/consolidate child securities instead");

            /* locate parcels that the split/consolidation applies to */
            var parcels = _ParcelService.GetParcels(stock, splitConsolidation.TransactionDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(splitConsolidation, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            foreach (ShareParcel parcel in parcels)
            {
                var newUnitCount = (int)Math.Round(parcel.Units * ((decimal)splitConsolidation.NewUnits / (decimal)splitConsolidation.OriginalUnits));
                ModifyParcel(unitOfWork, parcel, splitConsolidation.TransactionDate, ParcelEvent.SplitConsolidation, newUnitCount, parcel.CostBase, "");
            }

        }


    }
}
