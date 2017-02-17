using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Utils;

using StockManager.Service;

namespace PortfolioManager.Service
{

    public interface IPortfolioServiceLocator
    {
        T GetService<T>();
    }

    public abstract class PortfolioServiceLocator : IPortfolioServiceLocator
    {
        private readonly Dictionary<Type, object> _Services;
        private readonly Dictionary<Type, Func<object>> _ServiceFactories;

        public PortfolioServiceLocator()
        {
            _Services = new Dictionary<Type, object>();
            _ServiceFactories = new Dictionary<Type, Func<object>>();
        }

        public void Register<T>(Func<object> factory)
        {
            _ServiceFactories.Add(typeof(T), (Func<object>)factory);
        }

        public T GetService<T>()
        {
            object handler;

            if (!_Services.TryGetValue(typeof(T), out handler))
            {
                var factory = _ServiceFactories[typeof(T)];
                handler = factory();

                _Services.Add(typeof(T), handler);
            }

            return (T)handler;
        }
    }

    public class LocalPortfolioServiceLocator : PortfolioServiceLocator
    {
        private readonly PortfolioSettingsService _SettingsService;
        private readonly StockService _StockService;
        private readonly ParcelService _ParcelService;
        private readonly TransactionService _TransactionService;
        private readonly CashAccountService _CashAccountService;
        private readonly ShareHoldingService _ShareHoldingService;
        private readonly AttachmentService _AttachmentService;
        private readonly IncomeService _IncomeService;
        private readonly CGTService _CGTService;

        public LocalPortfolioServiceLocator(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            var stockServiceRepository = new StockServiceRepository(stockDatabase);
            var stockQuery = stockDatabase.StockQuery;
            var corporateActionQuery = stockDatabase.CorporateActionQuery;

            _SettingsService = new PortfolioSettingsService();

            _StockService = new StockService(stockServiceRepository);
            _ParcelService = new ParcelService(portfolioDatabase.PortfolioQuery, _StockService);
            _TransactionService = new TransactionService(portfolioDatabase, _ParcelService, _StockService, _AttachmentService);
            _CashAccountService = new CashAccountService(portfolioDatabase);
            _ShareHoldingService = new ShareHoldingService(_ParcelService, _StockService, _TransactionService, _CashAccountService);
            _AttachmentService = new AttachmentService(portfolioDatabase);
            _IncomeService = new IncomeService(portfolioDatabase.PortfolioQuery, _StockService, _SettingsService);
            _CGTService = new CGTService(portfolioDatabase.PortfolioQuery);

            Register<PortfolioSummaryService>(() => new PortfolioSummaryService(_ShareHoldingService, _CashAccountService));
            Register<PortfolioPerformanceService>(() => new PortfolioPerformanceService(_ShareHoldingService, _CashAccountService, _TransactionService, _StockService, _IncomeService));
        }

    }

   
}
