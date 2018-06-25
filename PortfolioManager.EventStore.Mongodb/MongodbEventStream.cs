using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStream : IEventStream
    {
        public Guid Id { get; private set; }

        private readonly string _ConnectionString;
        private readonly ILogger _Logger;

        public MongodbEventStream(Guid id, string connectionString, ILogger logger)
        {
            Id = id;

            _ConnectionString = connectionString;
            _Logger = logger;
        }

        public IEnumerable<Event> RetrieveEvents()
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Id.ToString());

            return collection.Find<Event>(x => true).ToList<Event>();
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Id.ToString());

            return collection.Find<Event>(x => x.EntityId == entityId).ToList<Event>();
        }

        public void StoreEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        public void StoreEvents(IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Id.ToString());

            collection.InsertMany(events);

            /*    var mongEvents = events.Select<IEvent, MongoEvent>(x => new MongoEvent() { Id = Guid.NewGuid(), AggregateId = x.Id, Version = x.Version, EventData = x });

                var collection = database.GetCollection<MongoEvent>(Id.ToString());

                collection.InsertMany(mongEvents); */
        }
    }

   /* class MongoEvent
    {
        public Guid Id { get; set; }      
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string EventType { get; set; }
        public IEvent EventData { get; set; }
    } */
}
