using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class SplitConsolidationHandler : ICorporateActionHandler
    {

        private readonly StockService _StockService;

        public SplitConsolidationHandler(StockService stockService) 
        {
            _StockService = stockService;
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction)
        {
            var splitConsolidation = corporateAction as SplitConsolidation;

            var transactions = new List<ITransaction>();

            var stock = _StockService.Get(splitConsolidation.Stock, splitConsolidation.ActionDate);

            transactions.Add(new UnitCountAdjustment()
            {
                ASXCode = stock.ASXCode,
                TransactionDate = splitConsolidation.ActionDate,
                OriginalUnits = splitConsolidation.OldUnits,
                NewUnits = splitConsolidation.NewUnits,
                Comment = splitConsolidation.Description
            }
            );

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(ICorporateAction corporateAction, TransactionService transactionService)
        {
            SplitConsolidation splitConsolidation = corporateAction as SplitConsolidation;
            string asxCode = _StockService.Get(splitConsolidation.Stock, splitConsolidation.ActionDate).ASXCode;

            var transactions = transactionService.GetTransactions(asxCode, TransactionType.UnitCountAdjustment, splitConsolidation.ActionDate, splitConsolidation.ActionDate);
            return (transactions.Count() > 0);
        }
    }
}
