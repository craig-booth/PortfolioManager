using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEventStore
    {
        void StoreEvent(Guid entityId, IEvent @event);

        IEnumerable<IEvent> RetrieveEvents();
        IEnumerable<IEvent> RetrieveEvents(Guid entityId);
    }

    public class InMemoryEventStore : IEventStore
    {
        private Dictionary<Guid, List<IEvent>> _Store = new Dictionary<Guid, List<IEvent>>();

        public void StoreEvent(Guid entityId, IEvent @event)
        {
            List<IEvent> events;
            if (_Store.ContainsKey(entityId))
            {
                events = _Store[entityId];
            }
            else
            {
                events = new List<IEvent>();
                _Store.Add(entityId, events);
            }

            events.Add(@event);
        }

        public IEnumerable<IEvent> RetrieveEvents()
        {
            foreach(var eventList in _Store.Values)
            {
                foreach (var @event in eventList)
                    yield return @event;
            }
        }

        public IEnumerable<IEvent> RetrieveEvents(Guid entityId)
        {
            return _Store[entityId];
        }
     }
}
