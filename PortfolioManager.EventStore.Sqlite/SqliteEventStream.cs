﻿using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PortfolioManager.EventStore.Sqlite
{
    class SqliteEventStream : IEventStream
    {
        public string Name { get; private set; }

        private readonly string _ConnectionString;
        private readonly ILogger _Logger;

        public SqliteEventStream(string name, string connectionString, ILogger logger)
        {
            Name = name;

            _ConnectionString = connectionString;
            _Logger = logger;
        }

        public IEnumerable<Event> RetrieveEvents()
        {
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();

                var sql = "SELECT [AggregateId], [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.Add("@StreamId", SqliteType.Text).Value = Name;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entityId = reader.GetString(0);
                            var eventType = reader.GetString(1);
                            var jsonData = reader.GetString(2);

                            var @event = (Event)JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));
                            @event.EntityId = new Guid(entityId);

                            yield return @event;
                        }
                        reader.Close();
                        _Logger?.LogInformation("RetrieveEvents Close(1)");
                    }
                }

                connection.Close();
            }
        }

        public IEnumerable<Event> RetrieveEvents(Guid entityId)
        {
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();

                var sql = "SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId and [AggregateId] = @AggregateId ORDER BY [Version]";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.Add("@StreamId", SqliteType.Text).Value = Name;
                    command.Parameters.Add("@AggregateId", SqliteType.Text).Value = entityId.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var eventType = reader.GetString(0);
                            var jsonData = reader.GetString(1);

                            var @event = (Event)JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));
                            @event.EntityId = entityId;

                            yield return @event;
                        }
                        reader.Close();
                        _Logger?.LogInformation("RetrieveEvents Close(2)");
                    }
                }

                connection.Close();
            }
        }

        public void StoreEvent(Event @event)
        {
            try
            {
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();

                    var sql = "INSERT2 INTO [Events] ([StreamId], [AggregateId], [Version], [EventType], [EventData]) VALUES (@StreamId, @AggregateId, @Version, @EventType, @EventData)";
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        var jsonData = JsonConvert.SerializeObject(@event);
                        var eventType = @event.GetType().AssemblyQualifiedName;

                        command.Parameters.Add("@StreamId", SqliteType.Text).Value = Name;
                        command.Parameters.Add("@AggregateId", SqliteType.Text).Value = @event.EntityId.ToString();
                        command.Parameters.Add("@Version", SqliteType.Integer).Value = @event.Version;
                        command.Parameters.Add("@EventType", SqliteType.Text).Value = eventType;
                        command.Parameters.Add("@EventData", SqliteType.Text).Value = jsonData;

                        _Logger?.LogInformation("Before");
                        _Logger?.LogInformation(command.ToString());
                        command.ExecuteNonQuery();
                        _Logger?.LogInformation("After");
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                _Logger?.LogError(e, "Error adding event to EventStore");
            }
        }

        public void StoreEvents(IEnumerable<Event> events)
        {
            try
            {
                using (var connection = new SqliteConnection(_ConnectionString))
                {
                    connection.Open();

                    var transaction = connection.BeginTransaction();

                    var sql = "INSERT INTO [Events] ([StreamId], [AggregateId], [Version], [EventType], [EventData]) VALUES (@StreamId, @AggregateId, @Version, @EventType, @EventData)";
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.Add("@StreamId", SqliteType.Text).Value = Name;
                        command.Parameters.Add("@AggregateId", SqliteType.Text);
                        command.Parameters.Add("@Version", SqliteType.Integer);
                        command.Parameters.Add("@EventType", SqliteType.Text);
                        command.Parameters.Add("@EventData", SqliteType.Text);

                        foreach (var @event in events)
                        {
                            var jsonData = JsonConvert.SerializeObject(@event);
                            var eventType = @event.GetType().AssemblyQualifiedName;

                            command.Parameters["@AggregateId"].Value = @event.EntityId.ToString();
                            command.Parameters["@Version"].Value = @event.Version;
                            command.Parameters["@EventType"].Value = eventType;
                            command.Parameters["@EventData"].Value = jsonData;
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                _Logger?.LogError(e, "Error adding event to EventStore");
            }

        }
    }
}
