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
         //   BsonSerializer.RegisterSerializer(typeof(DateTime), new DateOnlySerializer());
            var conventionPack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("PortfolioManager.Events", conventionPack, t => t.IsSubclassOf(typeof(Event)));

            var eventTypes = typeof(Event).GetSubclassesOf(true);
            foreach (var eventType in eventTypes)
                BsonClassMap.LookupClassMap(eventType);
        }

        public IEventStream<T> GetEventStream<T>(string streamName)
        {
            var eventStream = new MongodbEventStream<T>(streamName, _ConnectionString, _Logger);
            return (IEventStream<T>)eventStream;
        }
    }

}
