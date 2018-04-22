using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

using PortfolioManager.Domain;

namespace PortfolioManager.EventStore
{
    public class SqliteEventStore : IEventStore
    {
        private readonly SqliteTransactionFactory _TransationFactory;

        public SqliteEventStore(string databaseFile)
        {
            _TransationFactory = new SqliteTransactionFactory(databaseFile);

            CreateTables();
        }

        public IEventStream GetEventStream(Guid id)
        {
            return new SqliteEventStream(id, _TransationFactory);
        }

        private void CreateTables()
        {
            using (var transaction = _TransationFactory.CreateTransaction())
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
                        + " )", transaction.Connection, transaction);

                sqliteCommand.ExecuteNonQuery();
                transaction.Commit();
            }

        }
    }

    class SqliteTransactionFactory
    {
        private string _DatabaseFile;

        public SqliteTransactionFactory(string databaseFile)
        {
            _DatabaseFile = databaseFile;
        }

        public SqliteTransaction CreateTransaction()
        {
            var connection = new SqliteConnection("Data Source=" + _DatabaseFile);
            connection.Open();

            return connection.BeginTransaction();
        }
    }
}
