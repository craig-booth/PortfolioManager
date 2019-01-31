using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioManager.EventStore.Memory
{
    public class MemoryEventStream<T> : IEventStream<T>
    {
        private class StoredEntity
        {
            public Guid Id;
            public List<Event> Events = new List<Event>();
        }

        public string Collection { get; private set; }
        private Dictionary<Guid, StoredEntity> _Entities = new Dictionary<Guid, StoredEntity>();

        public MemoryEventStream(string collection)
        {
            Collection = collection;
        }

        public IEnumerable<Guid> GetStoredEntityIds()
        {
            return _Entities.Keys;
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            if (_Entities.ContainsKey(entityId))
            {
                var entity = _Entities[entityId];

                return entity.Events;
            }
            else
                return new Event[0];
        }

        public void StoreEvent(Guid entityId, Event @event)
        {
            StoredEntity entity;
            if (_Entities.ContainsKey(entityId))
                entity = _Entities[entityId];
            else
                entity = new StoredEntity() { Id = entityId };

            entity.Events.Add(@event); 
        }

        public void StoreEvents(Guid entityId, IEnumerable<Event> events)
        {

            StoredEntity entity;
            if (_Entities.ContainsKey(entityId))
                entity = _Entities[entityId];
            else
                entity = new StoredEntity() { Id = entityId };

            entity.Events.AddRange(events);
        }
    }
}
