using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.CorporateActions
{
    class CompositeActionHandler : ICorporateActionHandler
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly IStockQuery _StockQuery;
        private readonly ICorporateActionHandlerFactory _CorporateActionHandlerFactory;

        public CompositeActionHandler(IPortfolioQuery portfoliouery, IStockQuery stockQuery, ICorporateActionHandlerFactory corporateActionHandlerFactory)
        {
            _PortfolioQuery = portfoliouery;
            _StockQuery = stockQuery;
            _CorporateActionHandlerFactory = corporateActionHandlerFactory;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            CompositeAction compositeAction = corporateAction as CompositeAction;

            var transactions = new List<Transaction>();

            var stock = _StockQuery.Get(compositeAction.Stock, compositeAction.ActionDate);

            /* locate parcels that the capital return applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, compositeAction.ActionDate, compositeAction.ActionDate);
            if (!parcels.Any())
                return transactions;

            foreach (var childAction in compositeAction.Children)
            {
                var handler = _CorporateActionHandlerFactory.GetHandler(childAction);
                transactions.AddRange(handler.CreateTransactionList(childAction));
            }

            return transactions.AsReadOnly();
        }

        public bool HasBeenApplied(CorporateAction corporateAction)
        {
            CompositeAction compositeAction = corporateAction as CompositeAction;
            var childAction = compositeAction.Children[0];

            var handler = _CorporateActionHandlerFactory.GetHandler(childAction);

            return handler.HasBeenApplied(childAction);
        }

    }
}
