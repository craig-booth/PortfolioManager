﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Data.SQLite;

using PortfolioManager.Data.SQLite.Portfolios;

namespace PortfolioManager.Data.SQLite.Upgrade
{
    public abstract class SQLiteDatabaseUpgrade 
    {
        protected int _Version = -1;

        public int Version
        {
            get { return _Version; }
        }

        public abstract void Upgrade(SQLiteDatabase database);

        protected void ExecuteScript(SQLiteConnection connection, string fileName)
        {
            /* Load SQL commands to execute */
            var scriptFile = File.OpenText(fileName);
            var sqlCommands = scriptFile.ReadToEnd();
            scriptFile.Close();

            SQLiteCommand sqlCommand = new SQLiteCommand(sqlCommands, connection);
            sqlCommand.ExecuteNonQuery();
        }
    }

    public class SQLiteSimpleDatabaseUpgrade : SQLiteDatabaseUpgrade
    {
        private readonly string _ScriptFile;

        public SQLiteSimpleDatabaseUpgrade(int version, string scriptFile)
        {
            _Version = version;
            _ScriptFile = scriptFile;
        }

        public override void Upgrade(SQLiteDatabase database)
        {
            var transaction = database._Connection.BeginTransaction();

            database.ExecuteScript(_ScriptFile);

            transaction.Commit();
        }

    }

}