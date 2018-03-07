using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Transactions
{
    class CostBaseAdjustmentHandler : TransacactionHandler, ITransactionHandler
    {
        public CostBaseAdjustmentHandler(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
            : base (portfolioQuery, stockExchange)
        {

        }

        public void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var costBaseAdjustment = transaction as CostBaseAdjustment;

            var stock = _StockExchange.Stocks.Get(costBaseAdjustment.ASXCode, costBaseAdjustment.RecordDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(costBaseAdjustment, "Cannot adjust cost base of stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, costBaseAdjustment.RecordDate, costBaseAdjustment.RecordDate);

            if (!parcels.Any())
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
