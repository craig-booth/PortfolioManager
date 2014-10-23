using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;

namespace PortfolioManager.Data.Test
{
    public class TestBase
    {
        public IStockDatabase CreateStockDatabase()
        {
            return new SQLiteStockDatabase("Data Source=:memory:;Version=3;");

          //  return new SQLiteStockDatabase("Data Source=C:\\Users\\P150EM\\Desktop\\test.db;Version=3;");
        }

        public IPortfolioDatabase CreatePortfolioDatabase()
        {
            return new SQLitePortfolioDatabase("Data Source=:memory:;Version=3;");

          //  return new SQLitePortfolioDatabase("Data Source=C:\\Users\\craigb\\Desktop\\test.db;Version=3;");
        }

    }
}
