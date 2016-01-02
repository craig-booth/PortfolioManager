using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite
{
    public abstract class SQLiteDatabase
    {
        protected abstract int RepositoryVersion { get; }         

        protected internal SQLiteConnection _Connection;
        protected internal SQLiteRepositoryTransaction _Transaction;        

        public string FileName { get; private set; }
        public SQLiteDatabaseVersion Version { get; private set; }

        public SQLiteDatabase(string fileName)
        {
            FileName = fileName;
            Version = new SQLiteDatabaseVersion();

            _Connection = new SQLiteConnection("Data Source=" + FileName + ";Version=3;foreign keys=true;");
            _Connection.Open();

            var tableCount = new SQLiteCommand("SELECT count(*) FROM [sqlite_master]", _Connection);
            if ((long)tableCount.ExecuteScalar() == 0)
            {
                CreateDatabaseTables();

                Version.Version = RepositoryVersion;
                Version.CreationDateTime = DateTime.Today;
                Version.UpgradeDateTime = DateTime.Today;

                SetDbVersion();
            }
            else
            {
                LoadVersion();

                if (UpgradeRequired())
                    Upgrade();
            }

            _Transaction = new SQLiteRepositoryTransaction(_Connection);
        }

        private void LoadVersion()
        {            
            var sql = new SQLiteCommand("SELECT [Version], [CreationTime], [UpgradeTime] FROM [DbVersion]", _Connection);

            try
            {
                var reader = sql.ExecuteReader();

                if (reader.Read())
                {
                    Version.Version = reader.GetInt32(0);
                    Version.CreationDateTime = reader.GetDateTime(1);
                    Version.UpgradeDateTime = reader.GetDateTime(2);
                }
            }
            catch
            {
                Version.Version = 0;
                Version.CreationDateTime = DateTime.Today;
                Version.UpgradeDateTime = DateTime.Today;
            }                        
        }

        protected abstract void CreateDatabaseTables();

        protected virtual bool UpgradeRequired()
        {
            return (RepositoryVersion > Version.Version);
        }

        protected abstract SQLiteDatabaseUpgrade GetUpgrade(int forVersion);
        
        protected virtual void Upgrade()
        {
            // Backup the database first
            var backupFile = Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName) + DateTime.Now.ToString("_backup-yyyy-MM-dd") +Path.GetExtension(FileName));
            File.Copy(FileName, backupFile);

            while (UpgradeRequired())
            {
                var databaseUpgrade = GetUpgrade(Version.Version);                

                databaseUpgrade.Upgrade(this);

                Version.Version = databaseUpgrade.Version;
                Version.UpgradeDateTime = DateTime.Today;

                UpdateDbVersion();
            }
        }

        protected void UpdateDbVersion()
        {
            var updateCommand = new SQLiteCommand("UPDATE [DbVersion] SET [Version] = @Version, [UpgradeTime] = @UpgradeTime", _Connection);
            updateCommand.Prepare();

            updateCommand.Parameters.AddWithValue("@Version", Version.Version);
            updateCommand.Parameters.AddWithValue("@UpgradeTime", Version.UpgradeDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            updateCommand.ExecuteNonQuery();
        }

        protected void SetDbVersion()
        {
            var insertCommand = new SQLiteCommand("INSERT INTO [DbVersion] ([Version], [CreationTime], [UpgradeTime]) VALUES(@Version, @CreationTime, @UpgradeTime)", _Connection);
            insertCommand.Prepare();

            insertCommand.Parameters.AddWithValue("@Version", Version.Version);
            insertCommand.Parameters.AddWithValue("@CreationTime", Version.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
            insertCommand.Parameters.AddWithValue("@UpgradeTime", Version.UpgradeDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            insertCommand.ExecuteNonQuery();
        }

        protected internal void ExecuteScript(string fileName)
        {
            /* Load SQL commands to execute */
            var scriptFile = File.OpenText(fileName);
            var sqlScript = scriptFile.ReadToEnd();
            scriptFile.Close();

            var sqlCommands = sqlScript.Split(';');
            
            foreach (var sql in sqlCommands)
            {
                var sqlCommand = new SQLiteCommand(sql, _Connection);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
            }
        }
    }

    public class SQLiteDatabaseVersion
    {
        public int Version { get; internal set; }
        public DateTime CreationDateTime { get; internal set; }
        public DateTime UpgradeDateTime { get; internal set; }
    }
}
