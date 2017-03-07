using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Local
{
    class StockService : IStockService
    {
        private readonly Obsolete.StockService _StockService;

        public StockService(Obsolete.StockService stockService)
        {
            _StockService = stockService;
        }

        public Task<GetStockResponce> GetStocks(DateTime date, bool IncludeStapledSecurities, bool IncludeChildStocks)
        {
            var responce = new GetStockResponce();

            var stocks = _StockService.GetAll(date).AsEnumerable();

            if (!IncludeStapledSecurities)
                stocks = stocks.Where(x => x.Type != StockType.StapledSecurity);

            if (!IncludeChildStocks)
                stocks = stocks.Where(x => x.ParentId == Guid.Empty);

            responce.Stocks.AddRange(stocks.Select(x => new StockItem(x)));


            return Task.FromResult<GetStockResponce>(responce);
        }
    }
}
