using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace PortfolioManager.EventStore.Sqlite
{
    public class SqliteEventStore : IEventStore
    {
        private readonly ILogger _Logger;
        private readonly string _ConnectionString;

        public SqliteEventStore(string databaseFile)
        {
            _ConnectionString = "Data Source=" + databaseFile;

            CreateTables();
        }

        public SqliteEventStore(string databaseFile, ILogger<SqliteEventStore> logger)
            : this(databaseFile)
        {
            _Logger = logger;
        }

        public IEventStream GetEventStream(Guid id)
        {
            return new SqliteEventStream(id, _ConnectionString, _Logger);
        }

        private void CreateTables()
        {
            using (var connection = new SqliteConnection(_ConnectionString))
            {
                connection.Open();

                var sql = "CREATE TABLE IF NOT EXISTS [Events] "
                        + " ("
                        + " [StreamId] TEST(36) NOT NULL, "
                        + " [AggregateId] TEXT(36) NOT NULL, "
                        + " [Version] INTEGER NOT NULL, "
                        + " [EventType] TEXT(100) NOT NULL, "
                        + " [EventData] TEXT NOT NULL, "

                        + "PRIMARY KEY ([StreamId] ASC, [AggregateId] ASC, [Version] ASC)"
                        + " )";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

        }
    }
}
