using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.TradingCalanders
{
    public class TradingCalanderRepository :
        Repository<TradingCalander>,
        IRepository<TradingCalander>,
        ILoadableRepository<TradingCalander>
    {
        public TradingCalanderRepository(IEventStream<TradingCalander> eventStream, IEntityCache<TradingCalander> cache)
            : base(eventStream, cache)
        {
        }

        public void LoadFromEventStream()
        {
            var storedEntities = _EventStream.GetAll();
            foreach (var storedEntity in storedEntities)
            {
                var tradingCalander = new TradingCalander();
                tradingCalander.ApplyEvents(storedEntity.Events);
                _Cache.Add(tradingCalander);
            }
        }
    }
}
