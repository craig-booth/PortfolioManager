using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

using PortfolioManager.Domain;

namespace PortfolioManager.EventStore
{
    public class SqliteEventStore : IEventStore
    {
        private SqliteConnection _Connection;

        public SqliteEventStore(string fileName)
        {
            _Connection = new SqliteConnection("Data Source=" + fileName);
            _Connection.Open();

            CreateTables();
        }

        private SqliteCommand _InsertCommand = null;
        public void StoreEvent(Guid streamId, IEvent @event, int version)
        {
            if (_InsertCommand == null)
            {
                _InsertCommand = new SqliteCommand("INSERT INTO [Events] ([StreamId], [AggregateId], [Version], [EventType], [EventData]) VALUES (@StreamId, @AggregateId, @Version, @EventType, @EventData)", _Connection);
                _InsertCommand.Parameters.Add("@StreamId", SqliteType.Text);
                _InsertCommand.Parameters.Add("@AggregateId", SqliteType.Text);
                _InsertCommand.Parameters.Add("@Version", SqliteType.Integer);
                _InsertCommand.Parameters.Add("@EventType", SqliteType.Text);
                _InsertCommand.Parameters.Add("@EventData", SqliteType.Text);
                _InsertCommand.Prepare();
            }

            var jsonData = JsonConvert.SerializeObject(@event);

            _InsertCommand.Parameters["@StreamId"].Value = streamId.ToString();
            _InsertCommand.Parameters["@AggregateId"].Value = @event.Id.ToString();
            _InsertCommand.Parameters["@Version"].Value = version;
            _InsertCommand.Parameters["@EventType"].Value = @event.GetType().AssemblyQualifiedName;
            _InsertCommand.Parameters["@EventData"].Value = jsonData;
            _InsertCommand.ExecuteNonQuery();
        }

        private SqliteCommand _RetrieveAllCommand = null;
        public IEnumerable<Tuple<Guid, IEvent>> RetrieveEvents()
        {
            if (_RetrieveAllCommand == null)
            {
                _RetrieveAllCommand = new SqliteCommand("SELECT [StreamId], [EventType], [EventData] FROM [Events]", _Connection);
                _RetrieveAllCommand.Prepare();
            }
            var reader = _RetrieveAllCommand.ExecuteReader();
            while (reader.Read())
            {
                var streamId = new Guid(reader.GetString(0));
                var eventType = reader.GetString(1);
                var jsonData = reader.GetString(2);
          
                var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                yield return new Tuple<Guid, IEvent>(streamId, (IEvent)@event);
            }
            reader.Close();
        }

        private SqliteCommand _RetrieveStreamCommand = null;
        public IEnumerable<IEvent> RetrieveEvents(Guid streamId)
        {
            if (_RetrieveStreamCommand == null)
            {
                _RetrieveStreamCommand = new SqliteCommand("SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId", _Connection);
                _RetrieveStreamCommand.Parameters.Add("@StreamId", SqliteType.Text);
                _RetrieveStreamCommand.Parameters.Add("@AggregateId", SqliteType.Text);
                _RetrieveStreamCommand.Prepare();
            }

            _RetrieveStreamCommand.Parameters["@StreamId"].Value = streamId.ToString();
            var reader = _RetrieveStreamCommand.ExecuteReader();
            while (reader.Read())
            {
                var eventType = reader.GetString(0);
                var jsonData = reader.GetString(1);

                var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                yield return (IEvent)@event;
            }
            reader.Close();
        }

        private SqliteCommand _RetrieveEntityCommand = null;
        public IEnumerable<IEvent> RetrieveEvents(Guid streamId, Guid entityId)
        {
            if (_RetrieveEntityCommand == null)
            {
                _RetrieveEntityCommand = new SqliteCommand("SELECT [EventType], [EventData] FROM [Events] WHERE [StreamId] = @StreamId and [AggregateId] = @AggregateId ORDER BY [Version]", _Connection);
                _RetrieveEntityCommand.Parameters.Add("@StreamId", SqliteType.Text);
                _RetrieveEntityCommand.Parameters.Add("@AggregateId", SqliteType.Text);
                _RetrieveEntityCommand.Prepare();
            }

            _RetrieveEntityCommand.Parameters["@StreamId"].Value = streamId.ToString();
            _RetrieveEntityCommand.Parameters["@AggregateId"].Value = entityId.ToString();
            var reader = _RetrieveEntityCommand.ExecuteReader();
            while (reader.Read())
            {
                var eventType = reader.GetString(0);
                var jsonData = reader.GetString(1);

                var @event = JsonConvert.DeserializeObject(jsonData, Type.GetType(eventType));

                yield return (IEvent)@event;
            }
            reader.Close();
        }

        private void CreateTables()
        {
             var sqliteCommand = new SqliteCommand(
                "CREATE TABLE IF NOT EXISTS [Events] "
                    + " ("
                    + " [StreamId] TEST(36) NOT NULL, "
                    + " [AggregateId] TEXT(36) NOT NULL, "
                    + " [Version] INTEGER NOT NULL, "
                    + " [EventType] TEXT(100) NOT NULL, "
                    + " [EventData] TEXT NOT NULL, "

                    + "PRIMARY KEY ([StreamId] ASC, [AggregateId] ASC, [Version] ASC)"
                    + " )", _Connection);

            sqliteCommand.ExecuteNonQuery();
        }
    }
}
