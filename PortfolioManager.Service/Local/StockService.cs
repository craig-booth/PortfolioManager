using System;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Local
{
    public class StockService : IStockService
    { 
        private readonly IStockDatabase _StockDatabase;

        public StockService(IStockDatabase stockDatabase)
        {
            _StockDatabase = stockDatabase;
        }

        public Task<GetStockResponce> GetStocks(DateTime date, bool includeStapledSecurities, bool includeChildStocks)
        {
            var responce = new GetStockResponce();

            using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
            {
                var stocks = stockUnitOfWork.StockQuery.GetAll(date).AsEnumerable();

                if (!includeStapledSecurities)
                    stocks = stocks.Where(x => x.Type != StockType.StapledSecurity);

                if (!includeChildStocks)
                    stocks = stocks.Where(x => x.ParentId == Guid.Empty);

                responce.Stocks.AddRange(stocks.Select(x => new StockItem(x)));

            }

            return Task.FromResult<GetStockResponce>(responce);
        }
    }
}
