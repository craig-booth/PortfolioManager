using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class PortfolioValueService : IPortfolioValueService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public PortfolioValueService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<PortfolioValueResponce> GetPortfolioValue(DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IPortfolioValueService>();

            return await service.GetPortfolioValue(fromDate, toDate, frequency);
        }

        public async Task<PortfolioValueResponce> GetPortfolioValue(Guid stockId, DateTime fromDate, DateTime toDate, ValueFrequency frequency)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IPortfolioValueService>();

            return await service.GetPortfolioValue(stockId, fromDate, toDate, frequency);
        }
    }
}
