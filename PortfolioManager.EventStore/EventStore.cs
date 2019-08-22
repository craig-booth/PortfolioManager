using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioManager.EventStore
{
    public interface IEventStore
    {
        IEventStream GetEventStream(string collection);
        IEventStream<T> GetEventStream<T>(string collection);
    }


    public interface IEventStream
    {
        string Collection { get; }

        StoredEntity Get(Guid entityId);
        IEnumerable<StoredEntity> GetAll();

        StoredEntity FindFirst(string property, string value);
        IEnumerable<StoredEntity> Find(string property, string value);

        void Add(Guid entityId, string type, IEnumerable<Event> events);
        void Add(Guid entityId, string type, IDictionary<string,string> properties, IEnumerable<Event> events);

        void UpdateProperties(Guid entityId, IDictionary<string, string> properties);

        void AppendEvent(Guid entityId, Event @event);
        void AppendEvents(Guid entityId, IEnumerable<Event> events);
    }

    public interface IEventStream<T>
        : IEventStream
    {

    }

    public class StoredEntity
    {
        public Guid EntityId { get; set; }
        public string Type { get; set; }
        public int CurrentVersion { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
