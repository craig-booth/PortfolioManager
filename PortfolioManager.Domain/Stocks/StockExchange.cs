using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{

    public class StockExchange
    {
        public TradingCalander TradingCalander { get; private set; }
        public StockRepository Stocks { get; private set; }

        public StockExchange(IEventStore eventStore)
        {
            TradingCalander = new TradingCalander(eventStore.GetEventStream(TradingCalander.StreamId));
            Stocks = new StockRepository(eventStore.GetEventStream(StockRepository.StreamId));
        }

        public void LoadFromEventStream()
        {
            TradingCalander.LoadFromEventStream();
            Stocks.LoadFromEventStream();
        }
    }
}
