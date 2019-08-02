using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public class StockEntityFactory : IEntityFactory<Stock>
    {

        private readonly IRepository<StockPriceHistory> _StockPriceHistoryRepository;

        public StockEntityFactory(IRepository<StockPriceHistory> stockPriceHistoryRepository)
        {
            _StockPriceHistoryRepository = stockPriceHistoryRepository;
        }

        public Stock Create(Guid id, string storedEntityType)
        {
            if (storedEntityType == "Stock")
            {
                var stockPriceHistory =  _StockPriceHistoryRepository.Get(id);
                return new Stock(stockPriceHistory);
            }
            else if (storedEntityType == "StapledSecurity")
            {
                var stockPriceHistory = _StockPriceHistoryRepository.Get(id);
                return new StapledSecurity(stockPriceHistory);
            }
            else
                throw new NotSupportedException();
        }
    }
}
