using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    class CompositeActionHandler : ICorporateActionHandler
    {

        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;
        private readonly CorporateActionService _CorporateActionService;

        public CompositeActionHandler(StockService stockService, ParcelService parcelService, CorporateActionService corporateActionService)
        {
            _StockService = stockService;
            _ParcelService = parcelService;
            _CorporateActionService = corporateActionService;
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction)
        {
            CompositeAction compositeAction = corporateAction as CompositeAction;

            var transactions = new List<ITransaction>();

            var stock = _StockService.Get(compositeAction.Stock, compositeAction.ActionDate);

            /* locate parcels that the capital return applies to */
            var parcels = _ParcelService.GetParcels(stock, compositeAction.ActionDate);
            if (parcels.Count == 0)
                return transactions;

            foreach (var childAction in compositeAction.Children)
                transactions.AddRange(_CorporateActionService.CreateTransactionList(childAction));

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(ICorporateAction corporateAction, TransactionService transactionService)
        {
            CompositeAction compositeAction = corporateAction as CompositeAction;

            return _CorporateActionService.HasBeenApplied(compositeAction.Children[0]);
        }

    }
}
