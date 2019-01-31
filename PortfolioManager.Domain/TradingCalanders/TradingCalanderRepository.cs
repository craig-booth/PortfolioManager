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
            var entityIds = _EventStream.GetStoredEntityIds();
            foreach (var id in entityIds)
                Get(id);
        }
    }
}
