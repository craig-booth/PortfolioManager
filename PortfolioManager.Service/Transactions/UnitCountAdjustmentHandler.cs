﻿using System;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Transactions
{
    class UnitCountAdjustmentHandler : TransacactionHandler, ITransactionHandler
    {
        public UnitCountAdjustmentHandler(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
            : base (portfolioQuery, stockExchange)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var unitCountAdjustment = transaction as UnitCountAdjustment;

            var stock = _StockExchange.Stocks.Get(unitCountAdjustment.ASXCode, unitCountAdjustment.TransactionDate);

            if (stock is StapledSecurity)
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
