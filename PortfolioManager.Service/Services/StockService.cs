using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;
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

        public Task<CorporateActionsResponce> GetCorporateActions(Guid stockId, DateTime fromDate, DateTime toDate)
        {
            var responce = new CorporateActionsResponce();

            var stock = _StockExchange.Stocks.Get(stockId);
            var corporateActions = stock.CorporateActions.Values.Where(x => x.ActionDate.Between(fromDate, toDate)).Select(x => x.ToCorporateActionItem());
        
            responce.CorporateActions.AddRange(corporateActions);

            return Task.FromResult<CorporateActionsResponce>(responce);
        }
    }
}
