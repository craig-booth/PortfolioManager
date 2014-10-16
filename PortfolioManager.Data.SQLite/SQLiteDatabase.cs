using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite
{
    public class SQLiteDatabase
    {
        protected internal SQLiteConnection _Connection;
        protected internal SQLiteRepositoryTransaction _Transaction;

        public SQLiteDatabase(string connectionString)
        {
            _Connection = new SQLiteConnection(connectionString + ";foreign keys=true;");
            _Connection.Open();

            _Transaction = new SQLiteRepositoryTransaction(_Connection);

            var tableCount = new SQLiteCommand("SELECT count(*) FROM [sqlite_master]", _Connection);
            if ((long)tableCount.ExecuteScalar() == 0)
                CreateDatabaseTables();
        }

        protected virtual void CreateDatabaseTables()
        {
        }

        protected void CreateDatabaseTables(string fileName)
        {
            /* Load SQL commands to create database tables */
            var sqlCommandFile = File.OpenText(fileName);
            var sqlCommands = sqlCommandFile.ReadToEnd();
            sqlCommandFile.Close();

            SQLiteCommand createTableCommand = new SQLiteCommand(sqlCommands, _Connection);
            createTableCommand.ExecuteNonQuery();
        }
    }
}
