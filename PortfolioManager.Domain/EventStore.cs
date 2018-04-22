using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEventStore
    {
        IEventStream GetEventStream(Guid id);
    }

    public interface IEventStream
    {
        Guid Id { get; }
        void StoreEvent(IEvent @event);
        void StoreEvents(IEnumerable<IEvent> events);

        IEnumerable<IEvent> RetrieveEvents();
        IEnumerable<IEvent> RetrieveEvents(Guid entityId);
    }

}
