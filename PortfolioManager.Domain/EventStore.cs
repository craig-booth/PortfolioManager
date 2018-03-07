using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEventStore
    {
        void StoreEvent(Guid streamId, IEvent @event, int version);

        IEnumerable<Tuple<Guid, IEvent>> RetrieveEvents();
        IEnumerable<IEvent> RetrieveEvents(Guid streamId);
        IEnumerable<IEvent> RetrieveEvents(Guid streamId, Guid entityId);
    }

    public class InMemoryEventStore : IEventStore
    {
        private Dictionary<Guid, Dictionary<Guid, List<IEvent>>> _Store = new Dictionary<Guid, Dictionary<Guid, List<IEvent>>>();

        public void StoreEvent(Guid streamId, IEvent @event, int version)
        {
            Dictionary<Guid, List<IEvent>> eventStream;
            
            if (_Store.ContainsKey(streamId))
            {
                eventStream = _Store[streamId];
            }
            else
            {
                eventStream = new Dictionary<Guid, List<IEvent>>();
                _Store.Add(streamId, eventStream);
            }

            List<IEvent> entityStream;
            if (eventStream.ContainsKey(@event.Id))
            {
                entityStream = eventStream[@event.Id];
            }
            else
            {
                entityStream = new List<IEvent>();
                eventStream.Add(@event.Id, entityStream);
            }

            entityStream.Add(@event);
        }

        public IEnumerable<Tuple<Guid, IEvent>> RetrieveEvents()
        {
            foreach (var eventStream in _Store)
            {
                foreach (var entityStream in eventStream.Value.Values)
                {
                    foreach (var @event in entityStream)
                        yield return new Tuple<Guid, IEvent>(eventStream.Key, @event);
                }
            }
        }

        public IEnumerable<IEvent> RetrieveEvents(Guid streamId)
        {
            var eventStream = _Store[streamId];
            foreach (var entityStream in eventStream.Values)
            {
                foreach (var @event in entityStream)
                    yield return @event;
            }
        }

        public IEnumerable<IEvent> RetrieveEvents(Guid streamId, Guid entityId)
        {
            var eventStream = _Store[streamId];
            return eventStream[entityId];
        }
     }
}
