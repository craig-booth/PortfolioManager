using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.EventStore
{
    public interface IEventStore
    {
        IEventStream GetEventStream(Guid id);
    }

    public interface IEventStream
    {
        Guid Id { get; }
        void StoreEvent(Event @event);
        void StoreEvents(IEnumerable<Event> events);

        IEnumerable<Event> RetrieveEvents();
        IEnumerable<Event> RetrieveEvents(Guid entityId);
    }

    public static class EventStoreExtensions
    {
        public static void CopyEventStream(this IEventStore destinationStore, IEventStore sourceStore, Guid streamId)
        {
            var sourceStream = sourceStore.GetEventStream(streamId);
            var destinationStream = destinationStore.GetEventStream(streamId);

            destinationStream.StoreEvents(sourceStream.RetrieveEvents());
        }
    }

}
