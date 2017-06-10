using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    public class RestPortfolioManagerService : IPortfolioManagerService
    {
        private Local.LocalPortfolioManagerService _LocalPortfolioManagerService = new Local.LocalPortfolioManagerService();

        private ServiceFactory<IPortfolioService> _ServiceFactory = new ServiceFactory<IPortfolioService>();

        public RestPortfolioManagerService()
        {

        }

        public Task<bool> Connect(string portfolioDatabasePath, string stockDatabasePath)
        {
            _ServiceFactory.Clear();
            _ServiceFactory.Register<IPortfolioSummaryService>(() => new PortfolioSummaryService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<IPortfolioPerformanceService>(() => new PortfolioPerformanceService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<ICapitalGainService>(() => new CapitalGainService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<IPortfolioValueService>(() => new PortfolioValueService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<ICorporateActionService>(() => new CorporateActionService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<ITransactionService>(() => new TransactionService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<IHoldingService>(() => new HoldingService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<ICashAccountService>(() => new CashAccountService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<IIncomeService>(() => new IncomeService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<IStockService>(() => new StockService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));
            _ServiceFactory.Register<IAttachmentService>(() => new AttachmentService(_LocalPortfolioManagerService, portfolioDatabasePath, stockDatabasePath));

            return Task.FromResult<bool>(true);
        }

        public T GetService<T>() where T : IPortfolioService
        {
            return (T)_ServiceFactory.GetService<T>();
        }

        public async Task UpdateStockData(string stockDatabasePath)
        {
            await _LocalPortfolioManagerService.UpdateStockData(stockDatabasePath);
        }
    }
}
