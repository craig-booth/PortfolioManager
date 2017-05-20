using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.CorporateActions
{
    class CompositeActionHandler : ICorporateActionHandler
    {
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly StockService _StockService;
        private readonly ICorporateActionHandlerFactory _CorporateActionHandlerFactory;

        public CompositeActionHandler(IPortfolioQuery portfoliouery, StockService stockService, ICorporateActionHandlerFactory corporateActionHandlerFactory)
        {
            _PortfolioQuery = portfoliouery;
            _StockService = stockService;
            _CorporateActionHandlerFactory = corporateActionHandlerFactory;
        }

        public IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction)
        {
            CompositeAction compositeAction = corporateAction as CompositeAction;

            var transactions = new List<Transaction>();

            var stock = _StockService.Get(compositeAction.Stock, compositeAction.ActionDate);

            /* locate parcels that the capital return applies to */
            var parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, compositeAction.ActionDate, compositeAction.ActionDate);
            if (parcels.Count == 0)
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
