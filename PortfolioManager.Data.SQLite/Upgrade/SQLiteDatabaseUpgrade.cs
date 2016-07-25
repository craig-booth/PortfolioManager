using System;
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
            sqlCommand.Dispose();
        }
    }

    public class SQLiteSimpleDatabaseUpgrade : SQLiteDatabaseUpgrade
    {
        private readonly string _ScriptFile;
        private readonly string _CleanupScriptFile;

        public SQLiteSimpleDatabaseUpgrade(int version, string scriptFile)
            : this(version, scriptFile, "")
        {
        }

        public SQLiteSimpleDatabaseUpgrade(int version, string scriptFile, string cleanupScriptFile)
        {
            _Version = version;
            _ScriptFile = scriptFile;
            _CleanupScriptFile = cleanupScriptFile;
        }

        public override void Upgrade(SQLiteDatabase database)
        {
            SQLiteTransaction transaction;

            transaction = database._Connection.BeginTransaction();
            database.ExecuteScript(_ScriptFile);
            transaction.Commit();

            if (_CleanupScriptFile != "")
            {
                // Required to remove database locks
                database.Close();
                database.Open();

                transaction = database._Connection.BeginTransaction();
                database.ExecuteScript(_CleanupScriptFile);
                transaction.Commit();
            }
        }

    }

}
