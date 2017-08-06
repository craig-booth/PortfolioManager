using System;
using System.IO;
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite
{
    public abstract class SQLiteDatabase
    {
        protected abstract int RepositoryVersion { get; }              

        public string FileName { get; private set; }
        public SQLiteDatabaseVersion Version { get; private set; }

        public SQLiteDatabase(string fileName)
        {
            FileName = fileName;
            Version = new SQLiteDatabaseVersion();

            var connection = new SqliteConnection("Data Source=" + FileName);
            connection.Open();
            var transaction = connection.BeginTransaction();

            var tableCountCommand = new SqliteCommand("SELECT count(*) FROM [sqlite_master]", transaction.Connection, transaction);
            if ((long)tableCountCommand.ExecuteScalar() == 0)
            {
                CreateDatabase(transaction);
            }
            else
            {
                LoadVersion(transaction);

                if (UpgradeRequired())
                    Upgrade(transaction);
            }

            transaction.Commit();
            connection.Close();
        }

        private void LoadVersion(SqliteTransaction transaction)
        {            
            var sql = new SqliteCommand("SELECT [Version], [CreationTime], [UpgradeTime] FROM [DbVersion]", transaction.Connection, transaction);

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

        protected abstract void CreateDatabaseTables(SqliteTransaction transaction);

        protected virtual void CreateDatabase(SqliteTransaction transaction)
        {
            CreateDatabaseTables(transaction);

            Version.Version = RepositoryVersion;
            Version.CreationDateTime = DateTime.Today;
            Version.UpgradeDateTime = DateTime.Today;

            SetDbVersion(transaction);
        }

        protected virtual bool UpgradeRequired()
        {
            return (RepositoryVersion > Version.Version);
        }

        protected abstract SQLiteDatabaseUpgrade GetUpgrade(int forVersion);
        
        protected virtual void Upgrade(SqliteTransaction transaction)
        {
            // Backup the database first
            var backupFile = Path.Combine(Path.GetDirectoryName(FileName), Path.GetFileNameWithoutExtension(FileName) + DateTime.Now.ToString("_backup-yyyy-MM-dd") + Path.GetExtension(FileName));
            File.Copy(FileName, backupFile);

            while (UpgradeRequired())
            {
                var databaseUpgrade = GetUpgrade(Version.Version);                

                databaseUpgrade.Upgrade(this, transaction);

                Version.Version = databaseUpgrade.Version;
                Version.UpgradeDateTime = DateTime.Today;

                UpdateDbVersion(transaction);
            }
        }

        protected void UpdateDbVersion(SqliteTransaction transaction)
        {
            var updateCommand = new SqliteCommand("UPDATE [DbVersion] SET [Version] = @Version, [UpgradeTime] = @UpgradeTime", transaction.Connection, transaction);
            updateCommand.Prepare();

            updateCommand.Parameters.AddWithValue("@Version", Version.Version);
            updateCommand.Parameters.AddWithValue("@UpgradeTime", Version.UpgradeDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            updateCommand.ExecuteNonQuery();
        }

        protected void SetDbVersion(SqliteTransaction transaction)
        {
            var insertCommand = new SqliteCommand("INSERT INTO [DbVersion] ([Version], [CreationTime], [UpgradeTime]) VALUES(@Version, @CreationTime, @UpgradeTime)", transaction.Connection, transaction);
            insertCommand.Prepare();

            insertCommand.Parameters.AddWithValue("@Version", Version.Version);
            insertCommand.Parameters.AddWithValue("@CreationTime", Version.CreationDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
            insertCommand.Parameters.AddWithValue("@UpgradeTime", Version.UpgradeDateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

            insertCommand.ExecuteNonQuery();
        }

        protected internal void ExecuteScript(string fileName, SqliteTransaction transaction)
        {
            /* Load SQL commands to execute */
            using (var scriptFile = File.OpenText(fileName))
            {
                var sqlScript = scriptFile.ReadToEnd();

                var sqlCommand = new SqliteCommand(sqlScript, transaction.Connection, transaction);
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
