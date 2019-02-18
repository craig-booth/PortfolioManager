using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStream<T> :
        IEventStream,
        IEventStream<T>
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

        public StoredEntity Get(Guid entityId)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            var events = collection.Find<Event>(x => x.EntityId == entityId).SortBy(x => x.Version).ToList<Event>();

            return CreateEntity(entityId, events);
        }

        public IEnumerable<StoredEntity> GetAll()
        {
            var result = new List<StoredEntity>();

            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            var events = collection.Find<Event>(x => true).ToList<Event>();

            foreach (var eventGroup in events.GroupBy(x => x.EntityId))
            {
                result.Add(CreateEntity(eventGroup.Key, eventGroup.OrderBy(y => y.Version).ToList()));
            }

            return result;
        }

        private StoredEntity CreateEntity(Guid entityId, IList<Event> events)
        {
            var entity = new StoredEntity()
            {
                EntityId = entityId,
                CurrentVersion = events[events.Count - 1].Version + 1
            };

            entity.Events.AddRange(events);

            if (events[0].GetType().Name == "StockListedEvent")
                entity.Type = "Stock";
            else if (events[0].GetType().Name == "StapledSecurityListedEvent")
                entity.Type = "StapledSecurity";
            else if (events[0].GetType().Name == "NonTradingDaysSetEvent")
                entity.Type = "TradingCalander";

            return entity;

        }

        public void Add(Guid entityId, string type, IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            collection.InsertMany(events);
        }

        public void AppendEvent(Guid entityId, Event @event)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            collection.InsertOne(@event);
        }

        public void AppendEvents(Guid entityId, IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<Event>(Collection);

            collection.InsertMany(events);
        }

    }

    public class MongodbEventStream2<T> :
       IEventStream,
       IEventStream<T>
    {
        public string Collection { get; private set; }

        private readonly string _ConnectionString;
        private readonly ILogger _Logger;

        public MongodbEventStream2(string collection, string connectionString, ILogger logger)
        {
            Collection = collection;

            _ConnectionString = connectionString;
            _Logger = logger;
        }

        public StoredEntity Get(Guid entityId)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<StoredEntity>(Collection);

            var entity = collection.Find<StoredEntity>(x => x.EntityId == entityId).Single();

            return entity;
        }

        public IEnumerable<StoredEntity> GetAll()
        {
            var result = new List<StoredEntity>();

            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<StoredEntity>(Collection);

            var entities = collection.Find<StoredEntity>(x => true).ToList<StoredEntity>();

            return entities;
        }

        public void Add(Guid entityId, string type, IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<StoredEntity>(Collection);

            var entity = new StoredEntity()
            {
                EntityId = entityId,
                Type = type
            };
            entity.Events.AddRange(events);

            collection.InsertOne(entity);
        }

        public void AppendEvent(Guid entityId, Event @event)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<StoredEntity>(Collection);

            collection.FindOneAndUpdate<StoredEntity>(x => x.EntityId == entityId,
                Builders<StoredEntity>.Update.Push<Event>(x => x.Events, @event));
        }

        public void AppendEvents(Guid entityId, IEnumerable<Event> events)
        {
            var client = new MongoClient(_ConnectionString);
            var database = client.GetDatabase("PortfolioManager");

            var collection = database.GetCollection<StoredEntity>(Collection);

            collection.FindOneAndUpdate<StoredEntity>(x => x.EntityId == entityId,
                Builders<StoredEntity>.Update.PushEach<Event>(x => x.Events, events));
        }

    }
}
