using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

using PortfolioManager.Domain;

namespace PortfolioManager.EventStore
{
    public class MemoryEventStream : IEventStream
    {
        private class Event
        {
            public Guid EntityId;
            public Type EventType;
            public string EventData;
        }

        public Guid Id { get; private set; }
        private List<MemoryEventStream.Event> _Events;

        public MemoryEventStream(Guid id)
        {
            Id = id;
            _Events = new List<MemoryEventStream.Event>();
        }

        public IEnumerable<IEvent> RetrieveEvents()
        {
            foreach (var @event in _Events)
            {
                var result = JsonConvert.DeserializeObject(@event.EventData, @event.EventType);

                yield return (IEvent)result;
            }
        }

        public IEnumerable<IEvent> RetrieveEvents(Guid entityId)
        {
            foreach (var @event in _Events.Where(x => x.EntityId == entityId))
            {
                var result = JsonConvert.DeserializeObject(@event.EventData, @event.EventType);

                yield return (IEvent)result;
            }
        }

        public void StoreEvent(IEvent @event)
        {
            var jsonData = JsonConvert.SerializeObject(@event);


            _Events.Add(new MemoryEventStream.Event()
            {
                EntityId = @event.Id,
                EventType = @event.GetType(),
                EventData = jsonData
            });
        }

        public void StoreEvents(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
                StoreEvent(@event);
        }
    }
}
