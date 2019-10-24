﻿using System;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Booth.Common;

namespace PortfolioManager.EventStore.Mongodb
{
    public class MongodbEventStore : IEventStore
    {
        private readonly ILogger _Logger;
        private readonly string _ConnectionString;

        public MongodbEventStore(string connectionString)
        {
            _ConnectionString = connectionString;
            EventStoreSerializers.Register();
        }

        public MongodbEventStore(string databaseFile, ILogger<IEventStore> logger)
            : this(databaseFile)
        {
            _Logger = logger;
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

    static class EventStoreSerializers
    {
        private static bool _Registered = false;

        public static void Register()
        {
            if (_Registered)
                return;

            _Registered = true;

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
    }

}
