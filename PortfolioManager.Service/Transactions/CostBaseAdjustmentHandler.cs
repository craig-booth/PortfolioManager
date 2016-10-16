﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Transactions
{
    class CostBaseAdjustmentHandler : TransacactionHandler, ITransactionHandler
    {
        public CostBaseAdjustmentHandler(ParcelService parcelService, StockService stockService)
            : base (parcelService, stockService)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var costBaseAdjustment = transaction as CostBaseAdjustment;

            var stock = _StockService.Get(costBaseAdjustment.ASXCode, costBaseAdjustment.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(costBaseAdjustment, "Cannot adjust cost base of stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _ParcelService.GetParcels(stock, costBaseAdjustment.RecordDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(costBaseAdjustment, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            foreach (ShareParcel parcel in parcels)
            {
                var costBaseReduction = (parcel.CostBase * (1 - costBaseAdjustment.Percentage)).ToCurrency(RoundingRule.Round);
                ModifyParcel(unitOfWork, parcel, costBaseAdjustment.RecordDate, ParcelEvent.CostBaseReduction, x => { x.CostBase -= costBaseReduction; });
            }

        }
    }
}
