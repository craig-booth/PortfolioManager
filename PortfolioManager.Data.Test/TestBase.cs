using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit;
using NUnitExtension;


using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;


namespace PortfolioManager.Data.Test
{
    public class TestBase
    {
        public TestBase()
        {
            EntityConstraint.RegisterComparer(typeof(Stock));

            EntityConstraint.RegisterComparer(typeof(CapitalReturn));
            EntityConstraint.RegisterComparer(typeof(CompositeAction));
            EntityConstraint.RegisterComparer(typeof(Dividend));
            EntityConstraint.RegisterComparer(typeof(SplitConsolidation));
            EntityConstraint.RegisterComparer(typeof(Transformation));
        }

        public IStockDatabase CreateStockDatabase()
        {
            return new SQLiteStockDatabase(":memory:");

          //  return new SQLiteStockDatabase("Data Source=C:\\Users\\P150EM\\Desktop\\test.db;Version=3;");
        }

        public IPortfolioDatabase CreatePortfolioDatabase()
        {
            return new SQLitePortfolioDatabase(":memory:");

          //  return new SQLitePortfolioDatabase("Data Source=C:\\Users\\craigb\\Desktop\\test.db;Version=3;");
        }

    }
}
