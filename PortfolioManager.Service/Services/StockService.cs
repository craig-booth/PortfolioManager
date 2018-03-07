using System;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Services
{
    public class StockService : IStockService
    {
        private readonly StockExchange _StockExchange;

        public StockService(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public Task<GetStockResponce> GetStocks(DateTime date, bool includeStapledSecurities, bool includeChildStocks)
        {
            var responce = new GetStockResponce();

            responce.Stocks.AddRange(_StockExchange.Stocks.All(date).Select(x => x.ToStockItem(date)));

            return Task.FromResult<GetStockResponce>(responce); 
        }
    }
}
