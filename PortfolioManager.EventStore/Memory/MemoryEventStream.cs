using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PortfolioManager.EventStore.Memory
{
    public class MemoryEventStream : IEventStream
    {
        private class MemoryEvent
        {
            public Guid EntityId;
            public Type EventType;
            public string EventData;
        }

        public Guid Id { get; private set; }
        private List<MemoryEvent> _Events;

        public MemoryEventStream(Guid id)
        {
            Id = id;
            _Events = new List<MemoryEvent>();
        }

        public IEnumerable<Event> RetrieveEvents()
        {
            foreach (var @event in _Events)
            {
                var result = JsonConvert.DeserializeObject(@event.EventData, @event.EventType);

                yield return (Event)result;
            }
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            foreach (var @event in _Events.Where(x => x.EntityId == entityId))
            {
                var result = JsonConvert.DeserializeObject(@event.EventData, @event.EventType);

                yield return (Event)result;
            }
        }

        public void StoreEvent(Event @event)
        {
            var jsonData = JsonConvert.SerializeObject(@event);


            _Events.Add(new MemoryEvent()
            {
                EntityId = @event.EntityId,
                EventType = @event.GetType(),
                EventData = jsonData
            });
        }

        public void StoreEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
                StoreEvent(@event);
        }
    }
}
