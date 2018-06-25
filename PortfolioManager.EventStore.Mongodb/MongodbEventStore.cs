using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Bson.Serialization.Conventions;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStore : IEventStore
    {
        private readonly ILogger _Logger;
        private readonly string _ConnectionString;

        public MongodbEventStore(string connectionString)
        {
            _ConnectionString = connectionString;

            var conventionPack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("PortfolioManager.Events", conventionPack, t => t.IsSubclassOf(typeof(Event)));
        }

        public MongodbEventStore(string databaseFile, ILogger<MongodbEventStore> logger)
            : this(databaseFile)
        {
            _Logger = logger;
        }

        private void RegisterEventTypes() 
        {
            var eventTypes = typeof(Event).GetSubclassesOf(true);

            foreach (var eventType in eventTypes)
            {
                BsonClassMap.LookupClassMap(eventType);
            }
        }

        public IEventStream GetEventStream(string streamName)
        {
            return new MongodbEventStream(streamName, _ConnectionString, _Logger);
        }
    }

}
