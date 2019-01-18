using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.EventStore
{
    public interface ITrackedEntity
    {
        IEnumerable<Event> FetchEvents();
        void ApplyEvents(IEnumerable<Event> events);
    }
}
