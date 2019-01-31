using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.Portfolios;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Memory;

namespace PortfolioManager.Web
{
    public interface IPortfolioCache
    {
        Portfolio Get(Guid id);
    }

    public class PortfolioCache : IPortfolioCache
    {
        private IEventStore _EventStore;

        private Portfolio _Portfolio;

        public PortfolioCache()
        {
            _EventStore = new MemoryEventStore();
        }


        public Portfolio Get(Guid id)
        {
            if (_Portfolio == null)
                _Portfolio = new Portfolio(id, "Test", _EventStore.GetEventStream<Portfolio>("Portfolios"));

            return _Portfolio;
        }

    }
}
