using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.EventStore
{
    public interface IEventStore
    {
        IEventStream GetEventStream(string collection);
    }

    public interface IEventStream
    {
        string Collection { get; }

        IEnumerable<Guid> GetStoredEntityIds();

        void StoreEvent(Guid entityId, Event @event);
        void StoreEvents(Guid entityId, IEnumerable<Event> events);

        IEnumerable<Event> RetrieveEvents(Guid entityId);
    }

}
