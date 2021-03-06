﻿using System;
using System.IO;

using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioDatabase : SQLiteDatabase, IPortfolioDatabase
    {
        protected override int RepositoryVersion
        {
            get { return 10; }
        }

        public SQLitePortfolioDatabase(string fileName) 
            : base(fileName)
        {

        }

        protected override SQLiteDatabaseUpgrade GetUpgrade(int forVersion)
        {
            if (forVersion == 0)
                return new SQLiteSimpleDatabaseUpgrade(1, "Upgrade\\PortfolioDatabaseUpgradeToVersion1.sql");
            else if (forVersion == 1)
                return new SQLiteSimpleDatabaseUpgrade(2, "Upgrade\\PortfolioDatabaseUpgradeToVersion2.sql");
            else if (forVersion == 2)
                return new SQLiteSimpleDatabaseUpgrade(3, "Upgrade\\PortfolioDatabaseUpgradeToVersion3.sql");
            else if (forVersion == 3)
                return new SQLiteSimpleDatabaseUpgrade(4, "Upgrade\\PortfolioDatabaseUpgradeToVersion4.sql");
            else if (forVersion == 4)
                return new SQLiteSimpleDatabaseUpgrade(5, "Upgrade\\PortfolioDatabaseUpgradeToVersion5.sql");
            else if (forVersion == 5)
                return new SQLiteSimpleDatabaseUpgrade(6, "Upgrade\\PortfolioDatabaseUpgradeToVersion6.sql");
            else if (forVersion == 6)
                return new SQLiteSimpleDatabaseUpgrade(7, "Upgrade\\PortfolioDatabaseUpgradeToVersion7.sql");
            else if (forVersion == 7)
                return new SQLiteSimpleDatabaseUpgrade(8, "Upgrade\\PortfolioDatabaseUpgradeToVersion8.sql");
            else if (forVersion == 8)
                return new SQLiteSimpleDatabaseUpgrade(9, "Upgrade\\PortfolioDatabaseUpgradeToVersion9.sql");
            else if (forVersion == 9)
                return new SQLiteSimpleDatabaseUpgrade(10, "Upgrade\\PortfolioDatabaseUpgradeToVersion10.sql");
            else
                throw new NotSupportedException();
        }

        public IPortfolioUnitOfWork CreateUnitOfWork()
        {         
            return new SQLitePortfolioUnitOfWork(FileName);
        }

        public IPortfolioReadOnlyUnitOfWork CreateReadOnlyUnitOfWork()
        {
            return new SQLitePortfolioReadOnlyUnitOfWork(FileName);
        }

        protected override void CreateDatabaseTables(SqliteTransaction transaction)
        {
            var scriptFile = Path.Combine(AppContext.BaseDirectory, "Portfolio Database.sql");
            ExecuteScript(scriptFile, transaction);
        }
    }
}
