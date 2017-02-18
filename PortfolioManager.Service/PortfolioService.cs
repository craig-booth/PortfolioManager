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

        public LocalPortfolioServiceLocator(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            var stockServiceRepository = new StockServiceRepository(stockDatabase);
            var stockQuery = stockDatabase.StockQuery;
            var corporateActionQuery = stockDatabase.CorporateActionQuery;

            var settingsService = new PortfolioSettingsService();

            var stockService = new StockService(stockServiceRepository);
            var parcelService = new ParcelService(portfolioDatabase.PortfolioQuery, stockService);
            var attachmentService = new AttachmentService(portfolioDatabase);
            var transactionService = new TransactionService(portfolioDatabase, parcelService, stockService, attachmentService);
            var cashAccountService = new CashAccountService(portfolioDatabase);
            var shareHoldingService = new ShareHoldingService(parcelService, stockService, transactionService, cashAccountService);
            var incomeService = new IncomeService(portfolioDatabase.PortfolioQuery, stockService, settingsService);
            var cgtService = new CGTService2(portfolioDatabase.PortfolioQuery);

            Register<PortfolioSummaryService>(() => new PortfolioSummaryService(shareHoldingService, cashAccountService));
            Register<PortfolioPerformanceService>(() => new PortfolioPerformanceService(shareHoldingService, cashAccountService, transactionService, stockService, incomeService));
            Register<CGTService>(() => new CGTService(portfolioDatabase.PortfolioQuery, stockService, transactionService));
        }

    }

   
}
