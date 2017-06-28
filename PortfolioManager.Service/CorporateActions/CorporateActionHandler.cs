using System.Collections.Generic;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Service.CorporateActions
{
    interface ICorporateActionHandler
    {
        IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction);
        bool HasBeenApplied(CorporateAction corporateAction);
    }

    interface ICorporateActionHandlerFactory
    {
        ICorporateActionHandler GetHandler(CorporateAction corporateAction);
    }

    class CorporateActionHandlerFactory : ICorporateActionHandlerFactory
    {
        private ServiceFactory<ICorporateActionHandler> _HandlerFactory = new ServiceFactory<ICorporateActionHandler>();
        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly IStockQuery _StockQuery;

        public CorporateActionHandlerFactory(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _PortfolioQuery = portfolioQuery;
            _StockQuery = stockQuery;

            _HandlerFactory.Register<CapitalReturn>(() => new CapitalReturnHandler(_PortfolioQuery, _StockQuery));
            _HandlerFactory.Register<CompositeAction>(() => new CompositeActionHandler(_PortfolioQuery, _StockQuery, this));
            _HandlerFactory.Register<Dividend>(() => new DividendHandler(_PortfolioQuery, _StockQuery));
            _HandlerFactory.Register<SplitConsolidation>(() => new SplitConsolidationHandler(_PortfolioQuery, _StockQuery));
            _HandlerFactory.Register<Transformation>(() => new TransformationHandler(_PortfolioQuery, _StockQuery));
        }

        public ICorporateActionHandler GetHandler(CorporateAction corporateAction)
        {
            return _HandlerFactory.GetService(corporateAction);
        }
    }
}
