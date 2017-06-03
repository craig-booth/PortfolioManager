using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteLiveStockPriceDatabase : SQLiteDatabase, ILiveStockPriceDatabase
    {
        protected override int RepositoryVersion
        {
            get { return 1; }
        }

        public ILiveStockPriceUnitOfWork CreateUnitOfWork()
        {
            return new SQLiteLiveStockPriceUnitOfWork(this);
        }
        
        public ILiveStockPriceQuery LivePriceQuery { get; private set; }

        public SQLiteLiveStockPriceDatabase(string fileName) : base(fileName)
        {
            LivePriceQuery = new SQLiteLiveStockPriceQuery(this);
        }

        protected override SQLiteDatabaseUpgrade GetUpgrade(int forVersion)
        {
            throw new NotSupportedException();
        }

        protected override void CreateDatabaseTables()
        {
            ExecuteScript("LivePrice Database.sql");
        }


    }
}
