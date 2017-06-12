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
        protected int _Version;

        public int Version
        {
            get { return _Version; }
        }

        public SQLiteDatabaseUpgrade(int version)
        {
            _Version = version;
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

        public SQLiteSimpleDatabaseUpgrade(int version, string scriptFile)
            : base(version)
        {
            _ScriptFile = scriptFile;
        }

        public override void Upgrade(SQLiteDatabase database)
        {
            database.ExecuteScript(_ScriptFile);
        }
    }

}
