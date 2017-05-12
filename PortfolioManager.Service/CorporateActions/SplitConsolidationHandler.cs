using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.CorporateActions
{
    class SplitConsolidationHandler : ICorporateActionHandler
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly StockService _StockService;

        public SplitConsolidationHandler(IPortfolioQuery portfolioQuery, StockService stockService) 
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            var splitConsolidation = corporateAction as SplitConsolidation;

            var transactions = new List<Transaction>();

            var stock = _StockService.Get(splitConsolidation.Stock, splitConsolidation.ActionDate);

            /* locate parcels that the capital return applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, splitConsolidation.ActionDate, splitConsolidation.ActionDate);
            if (parcels.Count == 0)
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

        public bool HasBeenApplied(CorporateAction corporateAction, TransactionService transactionService)
        {
            SplitConsolidation splitConsolidation = corporateAction as SplitConsolidation;
            string asxCode = _StockService.Get(splitConsolidation.Stock, splitConsolidation.ActionDate).ASXCode;

            var transactions = transactionService.GetTransactions(asxCode, TransactionType.UnitCountAdjustment, splitConsolidation.ActionDate, splitConsolidation.ActionDate);
            return (transactions.Count() > 0);
        }
    }
}
