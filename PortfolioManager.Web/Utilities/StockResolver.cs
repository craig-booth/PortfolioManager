using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Web.Utilities
{
    public class StockResolver : IStockResolver
    {
        private readonly IEntityCache<Stock> _StockCache;

        public StockResolver(IEntityCache<Stock> stockCache)
        {
            _StockCache = stockCache;
        }

        public Stock GetStock(Guid id)
        {
            return _StockCache.Get(id);
        }
    }
}
