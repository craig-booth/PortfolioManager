using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks; 
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Data.Memory.Stocks;
using PortfolioManager.Data.Memory.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;

namespace PortfolioManager.Model.Test
{
    public class TestBase
    {
        public StockManager CreateStockManager()
        {
            //var database = new MemoryStockDatabase();
            var database = new SQLiteStockDatabase("Data Source=:memory:;Version=3;");

            return new StockManager(database);
        }

        public PortfolioManager.Model.Portfolios.PortfolioManager CreatePortfolioManager()
        {
            var stockDatabase = new MemoryStockDatabase();
            //var stockDatabase = new SQLiteStockDatabase("Data Source=:memory:;Version=3;");
            var portfolioDatabase = new MemoryPortfolioDatabase();
            //var portfolioDatabase = new SQLitePortfolioDatabase("Data Source=:memory:;Version=3;");

            return new PortfolioManager.Model.Portfolios.PortfolioManager(stockDatabase, portfolioDatabase);
        }

    }
}