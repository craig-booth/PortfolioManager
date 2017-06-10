using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class PortfolioPerformanceService : IPortfolioPerformanceService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public PortfolioPerformanceService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<PortfolioPerformanceResponce> GetPerformance(DateTime fromDate, DateTime toDate)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IPortfolioPerformanceService>();

            return await service.GetPerformance(fromDate, toDate);
        }
    }
}
