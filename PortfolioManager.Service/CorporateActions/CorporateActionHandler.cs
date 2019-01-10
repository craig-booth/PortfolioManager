using System.Collections.Generic;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Domain.CorporateActions;

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

        public CorporateActionHandlerFactory(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;

            _HandlerFactory.Register<CapitalReturn>(() => new CapitalReturnHandler(_PortfolioQuery))
                //      .Register<CompositeAction>(() => new CompositeActionHandler(_PortfolioQuery, _StockQuery, this))
                .Register<Dividend>(() => new DividendHandler(_PortfolioQuery));
           //     .Register<SplitConsolidation>(() => new SplitConsolidationHandler(_PortfolioQuery, _StockQuery))
           //     .Register<Transformation>(() => new TransformationHandler(_PortfolioQuery, _StockQuery));
        }

        public ICorporateActionHandler GetHandler(CorporateAction corporateAction)
        {
            return _HandlerFactory.GetService(corporateAction);
        }
    }
}
