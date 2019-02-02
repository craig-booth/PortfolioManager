using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioManager.EventStore.Memory
{
    public class MemoryEventStream<T> : IEventStream<T>
    {
        public string Collection { get; private set; }
        private Dictionary<Guid, StoredEntity> _Entities = new Dictionary<Guid, StoredEntity>();

        public MemoryEventStream(string collection)
        {
            Collection = collection;
        }

        public StoredEntity Get(Guid entityId)
        {
            if (_Entities.ContainsKey(entityId))
                return _Entities[entityId];
            else
                return null;
        }

        public IEnumerable<StoredEntity> GetAll()
        {
            return _Entities.Values;
        }

        public void Add(Guid entityId, string type, IEnumerable<Event> events)
        {
            var entity = new StoredEntity()
            {
                EntityId = entityId,
                Type = type,
                CurrentVersion = 0
            };

            entity.Events.AddRange(events);

            _Entities.Add(entityId, entity);
        }

        public void AppendEvent(Guid entityId, Event @event)
        {
            if (_Entities.ContainsKey(entityId))
            {
                var entity = _Entities[entityId];
                entity.Events.Add(@event);
            }
            else
                throw new KeyNotFoundException();
        }

        public void AppendEvents(Guid entityId, IEnumerable<Event> events)
        {
            if (_Entities.ContainsKey(entityId))
            {
                var entity = _Entities[entityId];
                entity.Events.AddRange(events);
            }
            else
                throw new KeyNotFoundException();
        }
    }
}
