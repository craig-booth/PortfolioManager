using System;
using System.IO;
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite
{
    public abstract class SQLiteDatabase
    {
        protected abstract int RepositoryVersion { get; }         

        protected internal SqliteConnection _Connection;
        protected internal SQLiteRepositoryTransaction _Transaction;        

        public string FileName { get; private set; }
        public SQLiteDatabaseVersion Version { get; private set; }

        public SQLiteDatabase(string fileName)
        {
            FileName = fileName;
            Version = new SQLiteDatabaseVersion();

            _Connection = new SqliteConnection("Data Source=" + FileName);
            _Connection.Open();

            _Transaction = new SQLiteRepositoryTransaction(_Connection);

               var tableCountCommand = new SqliteCommand("SELECT count(*) FROM [sqlite_master]", _Connection);
               if ((long)tableCountCommand.ExecuteScalar() == 0)
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
        }

        private void LoadVersion()
        {            
            var sql = new SqliteCommand("SELECT [Version], [CreationTime], [UpgradeTime] FROM [DbVersion]", _Connection);

            try
            {
                using (var reader = sql.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Version.Version = reader.GetInt32(0);
                        Version.CreationDateTime = reader.GetDateTime(1);
                        Version.UpgradeDateTime = reader.GetDateTime(2);
                    }
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
            var updateCommand = new SqliteCommand("UPDATE [DbVersion] SET [Version] = @Version, [UpgradeTime] = @UpgradeTime", _Connection);
            updateCommand.Prepare();

            updateCommand.Parameters.AddWithValue("@Version", Version.Version);
            updateCommand.Parameters.AddWithValue("@UpgradeTime", Version.UpgradeDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            updateCommand.ExecuteNonQuery();
        }

        protected void SetDbVersion()
        {
            var insertCommand = new SqliteCommand("INSERT INTO [DbVersion] ([Version], [CreationTime], [UpgradeTime]) VALUES(@Version, @CreationTime, @UpgradeTime)", _Connection);
            insertCommand.Prepare();

            insertCommand.Parameters.AddWithValue("@Version", Version.Version);
            insertCommand.Parameters.AddWithValue("@CreationTime", Version.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
            insertCommand.Parameters.AddWithValue("@UpgradeTime", Version.UpgradeDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            insertCommand.ExecuteNonQuery();
        }

        protected internal void ExecuteScript(string fileName)
        {
            /* Load SQL commands to execute */
            using (var scriptFile = File.OpenText(fileName))
            {
                var sqlScript = scriptFile.ReadToEnd();

                var sqlCommand = new SqliteCommand(sqlScript, _Connection);
                sqlCommand.ExecuteNonQuery();
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
