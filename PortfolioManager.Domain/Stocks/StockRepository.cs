using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public class StockRepository :
        Repository<Stock>,
        IRepository<Stock>,
        ILoadableRepository<Stock>
    {
        public StockRepository(IEventStream<Stock> eventStream, IEntityCache<Stock> cache)
            : base(eventStream, cache)
        {
        }

        public void LoadFromEventStream()
        {
            var storedEntities = _EventStream.GetAll();
            foreach (var storedEntity in storedEntities)
            {
                if (storedEntity.Type == "Stock")
                {
                    var stock = new Stock();
                    stock.ApplyEvents(storedEntity.Events);
                    _Cache.Add(stock);
                }
                else if (storedEntity.Type == "StapledSecurity")
                {
                    var stock = new StapledSecurity();
                    stock.ApplyEvents(storedEntity.Events);
                    _Cache.Add(stock);
                }
            }
        } 
    } 
}
