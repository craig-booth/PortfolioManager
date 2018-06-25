using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStream : IEventStream
    {
        public string Name { get; private set; }

        private readonly string _ConnectionString;
        private readonly ILogger _Logger;

        public MongodbEventStream(string name, string connectionString, ILogger logger)
        {
            Name = name;

            _ConnectionString = connectionString;
            _Logger = logger;
        }

        public IEnumerable<Event> RetrieveEvents()
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Name);

            return collection.Find<Event>(x => true).ToList<Event>();
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Name);

            return collection.Find<Event>(x => x.EntityId == entityId).ToList<Event>();
        }

        public void StoreEvent(Event @event)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Name);

            collection.InsertOne(@event);
        }

        public void StoreEvents(IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Name);

            collection.InsertMany(events);
        }
    }
}
