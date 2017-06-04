using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Utils;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Transactions
{
    class CostBaseAdjustmentHandler : TransacactionHandler, ITransactionHandler
    {
        public CostBaseAdjustmentHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
            : base (portfolioQuery, stockQuery)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var costBaseAdjustment = transaction as CostBaseAdjustment;

            var stock = _StockQuery.GetByASXCode(costBaseAdjustment.ASXCode, costBaseAdjustment.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(costBaseAdjustment, "Cannot adjust cost base of stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, costBaseAdjustment.RecordDate, costBaseAdjustment.RecordDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(costBaseAdjustment, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            foreach (ShareParcel parcel in parcels)
            {
                var costBaseReduction = (parcel.CostBase * (1 - costBaseAdjustment.Percentage)).ToCurrency(RoundingRule.Round);
                ReduceParcelCostBase(unitOfWork, parcel, costBaseAdjustment.RecordDate, costBaseReduction, transaction.Id);
            }
        }
    }
}
