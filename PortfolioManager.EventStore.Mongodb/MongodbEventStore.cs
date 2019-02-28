using System;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

using PortfolioManager.Common;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStore : IEventStore
    {
        private readonly ILogger _Logger;
        private readonly string _ConnectionString;

        public MongodbEventStore(string connectionString)
        {
            _ConnectionString = connectionString;
            RegisterEventTypes();
        }

        public MongodbEventStore(string databaseFile, ILogger<IEventStore> logger)
            : this(databaseFile)
        {
            _Logger = logger;
        }

        private void RegisterEventTypes() 
        {
            var conventionPack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("PortfolioManager.Events", conventionPack, t => t.IsSubclassOf(typeof(Event)));
            
            var eventTypes = typeof(Event).GetSubclassesOf(true);
            foreach (var eventType in eventTypes)
                BsonClassMap.LookupClassMap(eventType);
        }

        public IEventStream GetEventStream(string collection)
        {
            return GetEventStream<object>(collection);
        }

        public IEventStream<T> GetEventStream<T>(string streamName)
        {
            var eventStream = new MongodbEventStream<T>(streamName, _ConnectionString, _Logger);
            return (IEventStream<T>)eventStream;
        }
    }

    public class MongodbEventStore2 : IEventStore
    {
        private readonly ILogger _Logger;
        private readonly string _ConnectionString;

        public MongodbEventStore2(string connectionString)
        {
            _ConnectionString = connectionString;
            RegisterEventTypes();
        }

        public MongodbEventStore2(string databaseFile, ILogger<IEventStore> logger)
            : this(databaseFile)
        {
            _Logger = logger;
        }

        private void RegisterEventTypes()
        {
            var conventionPack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("PortfolioManager.Events", conventionPack, t => (t == typeof(StoredEntity)) ||  t.IsSubclassOf(typeof(Event)));

            var eventTypes = typeof(Event).GetSubclassesOf(true);
            foreach (var eventType in eventTypes)
                BsonClassMap.LookupClassMap(eventType);
        }

        public IEventStream GetEventStream(string collection)
        {
            return GetEventStream<object>(collection);
        }

        public IEventStream<T> GetEventStream<T>(string streamName)
        {
            var eventStream = new MongodbEventStream2<T>(streamName, _ConnectionString, _Logger);
            return (IEventStream<T>)eventStream;
        }
    }

    public class MongodbEventStore3 : IEventStore
    {
        private readonly ILogger _Logger;
        private readonly string _ConnectionString;

        public MongodbEventStore3(string connectionString)
        {
            _ConnectionString = connectionString;
            RegisterEventTypes();
        }

        public MongodbEventStore3(string databaseFile, ILogger<IEventStore> logger)
            : this(databaseFile)
        {
            _Logger = logger;
        }

        private void RegisterEventTypes()
        {
            BsonSerializer.RegisterSerializer(typeof(DateTime), new DateOnlySerializer());
            var conventionPack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("PortfolioManager.Events", conventionPack, t => (t == typeof(StoredEntity)) || t.IsSubclassOf(typeof(Event)));

            var eventTypes = typeof(Event).GetSubclassesOf(true);
            foreach (var eventType in eventTypes)
                BsonClassMap.LookupClassMap(eventType);
        }

        public IEventStream GetEventStream(string collection)
        {
            return GetEventStream<object>(collection);
        }

        public IEventStream<T> GetEventStream<T>(string streamName)
        {
            var eventStream = new MongodbEventStream2<T>(streamName, _ConnectionString, _Logger);
            return (IEventStream<T>)eventStream;
        }
    }

}
