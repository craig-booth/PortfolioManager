using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Utils
{
    class MoveStockPriceHistory
    {
        private IEventStream _StockEventStream;
        private IEventStream _StockPriceEventStream;

        public MoveStockPriceHistory(IEventStream stockEventStream, IEventStream stockPriceEventStream)
        {
            _StockEventStream = stockEventStream;
            _StockPriceEventStream = stockPriceEventStream;
        }

        public void MoveAll()
        {
            var cache = new EntityCache<StockPriceHistory>();
            var factory = new DefaultEntityFactory<StockPriceHistory>();
            var stockPriceRepository = new Repository<StockPriceHistory>(_StockPriceEventStream, cache, factory);
        }

        public void MoveStock()
        {

        }

    }
}
