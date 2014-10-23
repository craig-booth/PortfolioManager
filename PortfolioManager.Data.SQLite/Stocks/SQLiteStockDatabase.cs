using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockDatabase: SQLiteDatabase, IStockDatabase
    {
        public IStockUnitOfWork CreateUnitOfWork()
        {
            return new SQLiteStockUnitOfWork(this);
        }

        public IStockQuery StockQuery {get; private set;}
        public ICorporateActionQuery CorporateActionQuery { get; private set; }

        public SQLiteStockDatabase(string connectionString) : base(connectionString)
        {
            StockQuery = new SQLiteStockQuery(this);
            CorporateActionQuery = new SQLiteCorporateActionQuery(this);
        }

        protected override void CreateDatabaseTables()
        {
            CreateDatabaseTables("Stock Database.sql");
        }
    }

}
