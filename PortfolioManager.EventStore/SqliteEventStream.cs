using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using PortfolioManager.Domain;

namespace PortfolioManager.EventStore
{
    class SqliteEventStream : IEventStream
    {
        public Guid Id { get; private set; }

        private readonly string _ConnectionString;
        private readonly ILogger _Logger;

        public SqliteEventStream(Guid id, string connectionString, ILogger logger)
        {
            Id = id;

            _ConnectionString = connectionString;
            _Logger = logger;
        }

        public IEnumerable<IEvent> RetrieveEvents()
        {
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();

                var sql = "SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var eventType = reader.GetString(0);
                            var jsonData = reader.GetString(1);

                            var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                            yield return (IEvent)@event;
                        }
                        reader.Close();
                        _Logger?.LogInformation("RetrieveEvents Close(1)");
                    }
                }

                connection.Close();
            }
        }

        public IEnumerable<IEvent> RetrieveEvents(Guid entityId)
        {
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();

                var sql = "SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId and [AggregateId] = @AggregateId ORDER BY [Version]";
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();
                    command.Parameters.Add("@AggregateId", SqliteType.Text).Value = entityId.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var eventType = reader.GetString(0);
                            var jsonData = reader.GetString(1);

                            var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                            yield return (IEvent)@event;
                        }
                        reader.Close();
                        _Logger?.LogInformation("RetrieveEvents Close(2)");
                    }
                }

                connection.Close();
            }
        }

        public void StoreEvent(IEvent @event)
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

                        command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();
                        command.Parameters.Add("@AggregateId", SqliteType.Text).Value = @event.Id.ToString();
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

        public void StoreEvents(IEnumerable<IEvent> events)
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
                        command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();
                        command.Parameters.Add("@AggregateId", SqliteType.Text);
                        command.Parameters.Add("@Version", SqliteType.Integer);
                        command.Parameters.Add("@EventType", SqliteType.Text);
                        command.Parameters.Add("@EventData", SqliteType.Text);

                        foreach (var @event in events)
                        {
                            var jsonData = JsonConvert.SerializeObject(@event);
                            var eventType = @event.GetType().AssemblyQualifiedName;

                            command.Parameters["@AggregateId"].Value = @event.Id.ToString();
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
