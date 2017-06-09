using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class HoldingService : IHoldingService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public HoldingService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<HoldingResponce> GetHolding(Guid stockId, DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IHoldingService>();

            return await service.GetHolding(stockId, date);
        }

        public async Task<HoldingsResponce> GetHoldings(DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IHoldingService>();

            return await service.GetHoldings(date);
        }

        public async Task<HoldingsResponce> GetTradeableHoldings(DateTime date)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IHoldingService>();

            return await service.GetTradeableHoldings(date);
        }
    }
}
