using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{
    class HoldingService : IHoldingService
    {
        private readonly Obsolete.ShareHoldingService _ShareHoldingService;

        public HoldingService(Obsolete.ShareHoldingService shareHoldingService)
        {
            _ShareHoldingService = shareHoldingService;
        }

        public Task<GetOwnedStocksResponce> GetOwnedStocks(DateTime date)
        {
            var responce = new GetOwnedStocksResponce();

            var stocks = _ShareHoldingService.GetOwnedStocks(date, true);
            responce.Stocks.AddRange(stocks.Select(x => new StockItem(x.Id, x.ASXCode, x.Name)));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetOwnedStocksResponce>(responce);
        }
    }
}
