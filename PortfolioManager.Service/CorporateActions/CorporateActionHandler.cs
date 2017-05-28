using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

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
        private readonly Obsolete.StockService _StockService;

        public CorporateActionHandlerFactory(IPortfolioQuery portfolioQuery, IStockQuery stockQuery, Obsolete.StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockQuery = stockQuery;
            _StockService = stockService;

            _HandlerFactory.Register<CapitalReturn>(() => new CapitalReturnHandler(_PortfolioQuery, _StockQuery));
            _HandlerFactory.Register<CompositeAction>(() => new CompositeActionHandler(_PortfolioQuery, _StockService, this));
            _HandlerFactory.Register<Dividend>(() => new DividendHandler(_PortfolioQuery, _StockService));
            _HandlerFactory.Register<SplitConsolidation>(() => new SplitConsolidationHandler(_PortfolioQuery, _StockService));
            _HandlerFactory.Register<Transformation>(() => new TransformationHandler(_PortfolioQuery, _StockService));
        }

        public ICorporateActionHandler GetHandler(CorporateAction corporateAction)
        {
            return _HandlerFactory.GetService(corporateAction);
        }
    }
}
