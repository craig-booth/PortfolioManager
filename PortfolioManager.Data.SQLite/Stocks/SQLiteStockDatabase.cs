﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockDatabase: SQLiteDatabase, IStockDatabase
    {

        protected override int RepositoryVersion
        {
            get { return 1; }
        }

        public IStockUnitOfWork CreateUnitOfWork()
        {
            return new SQLiteStockUnitOfWork(this);
        }

        public IStockQuery StockQuery {get; private set;}
        public ICorporateActionQuery CorporateActionQuery { get; private set; }

        public SQLiteStockDatabase(string fileName) : base(fileName)
        {
            StockQuery = new SQLiteStockQuery(this);
            CorporateActionQuery = new SQLiteCorporateActionQuery(this);
        }

        protected override SQLiteDatabaseUpgrade GetUpgrade(int forVersion)
        {

            if (forVersion == 0)
                return new SQLiteSimpleDatabaseUpgrade(1, "Upgrade\\StockDatabaseUpgradeToVersion1.sql");
                throw new NotSupportedException();
        }

        protected override void CreateDatabaseTables()
        {
            ExecuteScript("Stock Database.sql");
        }
    }

}
