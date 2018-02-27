using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{

    public class StockExchange
    {
        private IEventStore _EventStore;

        public TradingCalander TradingCalander { get; private set; }
        public StockRepository Stocks { get; private set; }

        public StockExchange(IEventStore eventStore)
        {
            _EventStore = eventStore;
            TradingCalander = new TradingCalander(_EventStore);
            Stocks = new StockRepository(_EventStore);
        }

        public void Load()
        {
            var events = _EventStore.RetrieveEvents();
            foreach (var @event in events)
            {
                if (@event.GetType() == typeof(NonTradingDayAddedEvent))
                {
                    TradingCalander.Apply(@event as NonTradingDayAddedEvent);
                }
                else if (@event.GetType() == typeof(StockListedEvent))
                {
                    Stocks.Apply(@event as StockListedEvent);
                }
                else if (@event.GetType() == typeof(StockDelistedEvent))
                {
                    Stocks.Apply(@event as StockDelistedEvent);
                }
                else
                {
                    var stock = Stocks.Get(@event.Id);

                    dynamic dynamicEvent = @event;
                    stock.Apply(dynamicEvent);
                }
            }
        }
    }
}
