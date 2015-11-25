using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

namespace PortfolioManager.Data.SQLite.Upgrade
{

    class PortfolioDatabaseUpgradeToVersion1 : SQLiteDatabaseUpgrade
    {
        public PortfolioDatabaseUpgradeToVersion1()
        {
            _Version = 1;
        }

        public override void Upgrade(SQLiteDatabase database)
        {
            var transaction = database._Connection.BeginTransaction();

            database.ExecuteScript("Upgrade\\PortfolioDatabaseUpgradeToVersion1A.sql");

            // Move data from the backup to the new transactions table, defaulting sequence
            var insertCommand = new SQLiteCommand("INSERT INTO [Transactions] ([Id], [TransactionDate], [Sequence], [Type], [ASXCode], [Description]) VALUES (@Id, @TransactionDate, @Sequence, @Type, @ASXCode, @Description)", database._Connection);
            insertCommand.Prepare();

            var sql = new SQLiteCommand("SELECT [Id], [TransactionDate], [Type], [ASXCode], [Description] FROM [TransactionsBackup] ORDER BY [ASXCode], [TransactionDate]" , database._Connection);
            var reader = sql.ExecuteReader();

            string previousASXCode = "";
            string previousTransactionDate = "";
            int sequence = 0;
            while (reader.Read())
            {
                if ((reader.GetString(3) == previousASXCode) && (reader.GetString(1) == previousTransactionDate))
                {
                    sequence++;
                }
                else
                {
                    previousASXCode = reader.GetString(3);
                    previousTransactionDate = reader.GetString(1);
                    sequence = 0;
                }

                insertCommand.Parameters.AddWithValue("@Id", reader[0]);
                insertCommand.Parameters.AddWithValue("@TransactionDate", reader[1]);
                insertCommand.Parameters.AddWithValue("@Sequence", sequence);
                insertCommand.Parameters.AddWithValue("@Type", reader[2]);
                insertCommand.Parameters.AddWithValue("@ASXCode", reader[3]);
                insertCommand.Parameters.AddWithValue("@Description", reader[4]);

                insertCommand.ExecuteNonQuery();
            }

            database.ExecuteScript("Upgrade\\PortfolioDatabaseUpgradeToVersion1B.sql");

            transaction.Commit();
        }
    }
}
