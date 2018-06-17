using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

using PortfolioManager.Domain;

namespace PortfolioManager.EventStore
{
    class SqliteEventStream : IEventStream
    {
        public Guid Id { get; private set; }
        private SqliteEventStoreTransactionFactory _TransactionFactory;

        public SqliteEventStream(Guid id, SqliteEventStoreTransactionFactory transactionFactory)
        {
            Id = id;
            _TransactionFactory = transactionFactory;
        }

        public IEnumerable<IEvent> RetrieveEvents()
        {
            using (var transaction = _TransactionFactory.BeginTransaction())
            {
                var command = transaction.SqlCommand("SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId");
                command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var eventType = reader.GetString(0);
                    var jsonData = reader.GetString(1);

                    var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                    yield return (IEvent)@event;
                }
                reader.Close();
            }

        }

        public IEnumerable<IEvent> RetrieveEvents(Guid entityId)
        {
            using (var transaction = _TransactionFactory.BeginTransaction())
            {
                var command = transaction.SqlCommand("SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId and [AggregateId] = @AggregateId ORDER BY [Version]");
                command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();
                command.Parameters.Add("@AggregateId", SqliteType.Text).Value = entityId.ToString();

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var eventType = reader.GetString(0);
                    var jsonData = reader.GetString(1);

                    var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                    yield return (IEvent)@event;
                }
                reader.Close();
            }
        }

        public void StoreEvent(IEvent @event)
        {
            using (var transaction = _TransactionFactory.BeginTransaction())
            {
                var jsonData = JsonConvert.SerializeObject(@event);

                var command = transaction.SqlCommand("INSERT INTO [Events] ([StreamId], [AggregateId], [Version], [EventType], [EventData]) VALUES (@StreamId, @AggregateId, @Version, @EventType, @EventData)");
                command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();
                command.Parameters.Add("@AggregateId", SqliteType.Text).Value = @event.Id.ToString();
                command.Parameters.Add("@Version", SqliteType.Integer).Value = @event.Version;
                command.Parameters.Add("@EventType", SqliteType.Text).Value = @event.GetType().AssemblyQualifiedName;
                command.Parameters.Add("@EventData", SqliteType.Text).Value = jsonData;
                command.ExecuteNonQuery();

                transaction.Commit();
            }

        }

        public void StoreEvents(IEnumerable<IEvent> events)
        {
            using (var transaction = _TransactionFactory.BeginTransaction())
            {
                var command = transaction.SqlCommand("INSERT INTO [Events] ([StreamId], [AggregateId], [Version], [EventType], [EventData]) VALUES (@StreamId, @AggregateId, @Version, @EventType, @EventData)");
                command.Parameters.Add("@StreamId", SqliteType.Text).Value = Id.ToString();
                command.Parameters.Add("@AggregateId", SqliteType.Text);
                command.Parameters.Add("@Version", SqliteType.Integer);
                command.Parameters.Add("@EventType", SqliteType.Text);
                command.Parameters.Add("@EventData", SqliteType.Text);
                
                foreach (var @event in events)
                {
                    var jsonData = JsonConvert.SerializeObject(@event);

                    command.Parameters["@AggregateId"].Value = @event.Id.ToString();
                    command.Parameters["@Version"].Value = @event.Version;
                    command.Parameters["@EventType"].Value = @event.GetType().AssemblyQualifiedName;
                    command.Parameters["@EventData"].Value = jsonData;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }


        }
    }
}
