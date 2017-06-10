using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class CorporateActionService : ICorporateActionService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public CorporateActionService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions()
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICorporateActionService>();

            return await service.GetUnappliedCorporateActions();
        }

        public async Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid actionId)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ICorporateActionService>();

            return await service.TransactionsForCorporateAction(actionId);
        }
    }
}
