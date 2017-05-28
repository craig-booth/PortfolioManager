using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Local
{
    class StockService : IStockService
    {
        private readonly IStockQuery _StockQuery;

        public StockService(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
        }

        public Task<GetStockResponce> GetStocks(DateTime date, bool IncludeStapledSecurities, bool IncludeChildStocks)
        {
            var responce = new GetStockResponce();

            var stocks = _StockQuery.GetAll(date).AsEnumerable();

            if (!IncludeStapledSecurities)
                stocks = stocks.Where(x => x.Type != StockType.StapledSecurity);

            if (!IncludeChildStocks)
                stocks = stocks.Where(x => x.ParentId == Guid.Empty);

            responce.Stocks.AddRange(stocks.Select(x => new StockItem(x)));


            return Task.FromResult<GetStockResponce>(responce);
        }
    }
}
