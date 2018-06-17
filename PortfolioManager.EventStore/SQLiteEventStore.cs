using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using PortfolioManager.Domain;

namespace PortfolioManager.EventStore
{
    public class SqliteEventStore : IEventStore
    {
        private readonly ILogger logger;
        private readonly SqliteEventStoreTransactionFactory _TransationFactory;

        public SqliteEventStore(string databaseFile)
        {
            _TransationFactory = new SqliteEventStoreTransactionFactory(databaseFile);

            CreateTables();
        }

        public SqliteEventStore(string databaseFile, ILogger<SqliteEventStore> logger)
            : this(databaseFile)
        {

        }

        public IEventStream GetEventStream(Guid id)
        {
            return new SqliteEventStream(id, _TransationFactory);
        }

        private void CreateTables()
        {
            using (var transaction = _TransationFactory.BeginTransaction())
            {
                var command = transaction.SqlCommand(
                    "CREATE TABLE IF NOT EXISTS [Events] "
                        + " ("
                        + " [StreamId] TEST(36) NOT NULL, "
                        + " [AggregateId] TEXT(36) NOT NULL, "
                        + " [Version] INTEGER NOT NULL, "
                        + " [EventType] TEXT(100) NOT NULL, "
                        + " [EventData] TEXT NOT NULL, "

                        + "PRIMARY KEY ([StreamId] ASC, [AggregateId] ASC, [Version] ASC)"
                        + " )");
                command.ExecuteNonQuery();

                transaction.Commit();
            }

        }
    }

    class SqliteEventStoreTransactionFactory
    {
        private string _DatabaseFile;

        public SqliteEventStoreTransactionFactory(string databaseFile)
        {
            _DatabaseFile = databaseFile;
        }

        public SQLiteEventStoreTransaction BeginTransaction()
        {
            return new SQLiteEventStoreTransaction(_DatabaseFile);
        }
    }

    class SQLiteEventStoreTransaction : IDisposable
    {
        private SqliteConnection _Connection;
        private SqliteTransaction _Transaction;

        public SQLiteEventStoreTransaction(string databaseFile)
        {
            _Connection = new SqliteConnection("Data Source=" + databaseFile);
            _Connection.Open();

            _Transaction = _Connection.BeginTransaction();
        }

        public SqliteCommand SqlCommand(string sql)
        {
            return new SqliteCommand(sql, _Connection, _Transaction);           
        }

        public void Commit()
        {
            _Transaction.Commit();
            _Transaction = null;
        }

        public void Dispose()
        {
            if (_Transaction != null)
                _Transaction.Rollback();

            _Connection.Close();
        }
    }
}
