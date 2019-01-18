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

        public string Collection { get; private set; }
        private List<MemoryEvent> _Events;

        public MemoryEventStream(string collection)
        {
            Collection = collection;
            _Events = new List<MemoryEvent>();
        }

        public IEnumerable<Guid> GetStoredEntityIds()
        {
            return _Events.GroupBy(x => x.EntityId).Select(y => y.Key);
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            foreach (var @event in _Events.Where(x => x.EntityId == entityId))
            {
                var result = JsonConvert.DeserializeObject(@event.EventData, @event.EventType);

                yield return (Event)result;
            }
        }

        public void StoreEvent(Guid entityId, Event @event)
        {
            var jsonData = JsonConvert.SerializeObject(@event);

            _Events.Add(new MemoryEvent()
            {
                EntityId = entityId,
                EventType = @event.GetType(),
                EventData = jsonData
            });
        }

        public void StoreEvents(Guid entityId, IEnumerable<Event> events)
        {
            foreach (var @event in events)
                StoreEvent(entityId, @event);
        }
    }
}
