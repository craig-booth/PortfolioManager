using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;


namespace PortfolioManager.Service.CorporateActions
{
    class SplitConsolidationHandler : ICorporateActionHandler
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly IStockQuery _StockQuery;

        public SplitConsolidationHandler(IPortfolioQuery portfolioQuery, IStockQuery stockQuery) 
        {
            _PortfolioQuery = portfolioQuery;
            _StockQuery = stockQuery;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var splitConsolidation = corporateAction as SplitConsolidation;

            var transactions = new List<Transaction>();

            var stock = _StockQuery.Get(splitConsolidation.Stock, splitConsolidation.ActionDate);

            /* locate parcels that the capital return applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, splitConsolidation.ActionDate, splitConsolidation.ActionDate);
            if (!parcels.Any())
                return transactions;

            transactions.Add(new UnitCountAdjustment()
            {
                ASXCode = stock.ASXCode,
                TransactionDate = splitConsolidation.ActionDate,
                OriginalUnits = splitConsolidation.OldUnits,
                NewUnits = splitConsolidation.NewUnits,
                RecordDate = splitConsolidation.ActionDate,
                Comment = splitConsolidation.Description
            }
            );

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(CorporateAction corporateAction)
        {
            SplitConsolidation splitConsolidation = corporateAction as SplitConsolidation;

            string asxCode = _StockQuery.Get(splitConsolidation.Stock, splitConsolidation.ActionDate).ASXCode;

            var transactions = _PortfolioQuery.GetTransactions(asxCode, TransactionType.UnitCountAdjustment, splitConsolidation.ActionDate, splitConsolidation.ActionDate);
            return (transactions.Count() > 0);
        }
    }
}
