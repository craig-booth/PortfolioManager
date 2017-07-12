using System;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Upgrade;


namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockDatabase: SQLiteDatabase, IStockDatabase
    {

        protected override int RepositoryVersion
        {
            get { return 6; }
        }

        public IStockUnitOfWork CreateUnitOfWork()
        {
            var transaction = CreateTransaction();

            return new SQLiteStockUnitOfWork(transaction);
        }

        public IStockQuery StockQuery
        {
            get
            {
                var transaction = CreateTransaction();
                return new SQLiteStockQuery(transaction);
            }
        }

        public ICorporateActionQuery CorporateActionQuery
        {
            get
            {
                var transaction = CreateTransaction();
                return new SQLiteCorporateActionQuery(transaction);
            }
        }

        public SQLiteStockDatabase(string fileName) 
            : base(fileName)
        {

        }

        protected override SQLiteDatabaseUpgrade GetUpgrade(int forVersion)
        {

            if (forVersion == 0)
                return new SQLiteSimpleDatabaseUpgrade(1, "Upgrade\\StockDatabaseUpgradeToVersion1.sql");
            else if (forVersion == 1)
                return new SQLiteSimpleDatabaseUpgrade(2, "Upgrade\\StockDatabaseUpgradeToVersion2.sql");
            else if (forVersion == 2)
                return new SQLiteSimpleDatabaseUpgrade(3, "Upgrade\\StockDatabaseUpgradeToVersion3.sql");
            else if (forVersion == 3)
                return new SQLiteSimpleDatabaseUpgrade(4, "Upgrade\\StockDatabaseUpgradeToVersion4.sql");
            else if (forVersion == 4)
                return new SQLiteSimpleDatabaseUpgrade(5, "Upgrade\\StockDatabaseUpgradeToVersion5.sql");
            else if (forVersion == 5)
                return new SQLiteSimpleDatabaseUpgrade(6, "Upgrade\\StockDatabaseUpgradeToVersion6.sql");
            else
                throw new NotSupportedException();
        }

        protected override void CreateDatabaseTables(SqliteTransaction transaction)
        {
            ExecuteScript("Stock Database.sql", transaction);
        }
    }

}
