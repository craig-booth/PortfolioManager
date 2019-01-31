using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStream<T> : IEventStream<T>
    {
        public string Collection { get; private set; }

        private readonly string _ConnectionString;
        private readonly ILogger _Logger;

        public MongodbEventStream(string collection, string connectionString, ILogger logger)
        {
            Collection = collection;

            _ConnectionString = connectionString;
            _Logger = logger;
        }

        public IEnumerable<Guid> GetStoredEntityIds()
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            if (typeof(T).Name == "Stock")
            {
                var events = collection.Find<Event>(x => true).ToList();
                return events.Where(x => x.GetType().Name == "StockListedEvent").Select(x => x.EntityId);
            }
            else if (typeof(T).Name == "StapledSecurity")
            {
                var events = collection.Find<Event>(x => true).ToList();
                return events.Where(x => x.GetType().Name == "StapledSecurityListedEvent").Select(x => x.EntityId);
            }
            else
                return collection.AsQueryable<Event>().Select(x => x.EntityId).Distinct();
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            return collection.Find<Event>(x => x.EntityId == entityId).SortBy(x => x.Version).ToList<Event>();
        }

        public void StoreEvent(Guid entityId, Event @event)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            collection.InsertOne(@event);
        }

        public void StoreEvents(Guid entityId, IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            collection.InsertMany(events);
        }
    }
}
