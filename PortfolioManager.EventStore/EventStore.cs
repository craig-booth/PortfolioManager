using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.EventStore
{
    public interface IEventStore
    {
        IEventStream GetEventStream(string streamName);
    }

    public interface IEventStream
    {
        string Name { get; }
        void StoreEvent(Event @event);
        void StoreEvents(IEnumerable<Event> events);

        IEnumerable<Event> RetrieveEvents();
        IEnumerable<Event> RetrieveEvents(Guid entityId);
    }

    public static class EventStoreExtensions
    {
        public static void CopyEventStream(this IEventStore destinationStore, IEventStore sourceStore, string streamName)
        {
            var sourceStream = sourceStore.GetEventStream(streamName);
            var destinationStream = destinationStore.GetEventStream(streamName);

            destinationStream.StoreEvents(sourceStream.RetrieveEvents());
        }
    }

}
