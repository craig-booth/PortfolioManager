using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class StockService : IStockService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public StockService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }
        

        public async Task<GetStockResponce> GetStocks(DateTime date, bool includeStapledSecurities, bool includeChildStocks)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<IStockService>();

            return await service.GetStocks(date, includeStapledSecurities, includeChildStocks);
        }
    }

}
