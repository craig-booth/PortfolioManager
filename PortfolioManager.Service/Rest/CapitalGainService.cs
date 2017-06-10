using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class CapitalGainService : ICapitalGainService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public CapitalGainService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<CGTLiabilityResponce> GetCGTLiability(DateTime fromDate, DateTime toDate)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICapitalGainService>();

            return await service.GetCGTLiability(fromDate, toDate);
        }

        public async Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICapitalGainService>();

            return await service.GetDetailedUnrealisedGains(date);
        }

        public async Task<DetailedUnrealisedGainsResponce> GetDetailedUnrealisedGains(Guid stockId, DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICapitalGainService>();

            return await service.GetDetailedUnrealisedGains(stockId, date);
        }

        public async Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICapitalGainService>();

            return await service.GetSimpleUnrealisedGains(date);
        }

        public async Task<SimpleUnrealisedGainsResponce> GetSimpleUnrealisedGains(Guid stockId, DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICapitalGainService>();

            return await service.GetSimpleUnrealisedGains(stockId, date);
        }
    }
}
