using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain
{
    public interface ITrackedEntity : IEntity
    {
        IEnumerable<Event> FetchEvents();
        void ApplyEvents(IEnumerable<Event> events);
    }
}
