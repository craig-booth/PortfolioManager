using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{

    public class StockExchange
    {
        public static readonly Guid StreamId = new Guid("0C295D02-FCB3-4FBA-8134-8C53E4446972");


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
                var streamId = @event.Item1;
                dynamic dynamicEvent = @event.Item2;

                if (streamId == TradingCalander.StreamId)
                {
                    TradingCalander.Apply(dynamicEvent);
                }
                else if (streamId == StockRepository.StreamId)
                {
                    Stocks.Apply(dynamicEvent);
                }
                else if (streamId == Stock.StreamId)
                {
                    var stock = Stocks.Get(dynamicEvent.Id);
                    stock.Apply(dynamicEvent);
                }
            }
        }
    }
}
