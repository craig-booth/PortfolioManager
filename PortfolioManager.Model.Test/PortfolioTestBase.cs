using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Data.Memory.Stocks;
using PortfolioManager.Data.Memory.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;

namespace PortfolioManager.Model.Test
{
    public class PortfolioTestBase
    {
        private IStockDatabase _StockDatabase;

        protected Guid _AAAId;
        protected Guid _BBBId;
        protected Guid _CCCId;
        protected Guid _SSSId;
        protected Guid _SSS1Id;
        protected Guid _SSS2Id;
        protected Guid _SSS3Id;

        public PortfolioTestBase()
        {
            _StockDatabase = new SQLiteStockDatabase("Data Source=:memory:;Version=3;");
            var stockManager = new StockManager(_StockDatabase);
            AddTestStocks(stockManager);
        }


        public StockManager CreateStockManager()
        {
            var stockDatabase = new SQLiteStockDatabase("Data Source=:memory:;Version=3;");

            return new StockManager(stockDatabase);
        }

        public PortfolioManager.Model.Portfolios.PortfolioManager CreatePortfolioManager()
        {
            var portfolioDatabase = new SQLitePortfolioDatabase("Data Source=:memory:;Version=3;");

            return new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);
        }

        public Portfolio CreateTestPortfolio()
        {
            var portfolioDatabase = new SQLitePortfolioDatabase("Data Source=:memory:;Version=3;");
            var portfolioManager = new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);

            return portfolioManager.CreatePortfolio("Test portfolio");
        }

        public void AddTestStocks(StockManager stockManager)
        {
            var aaa = stockManager.Add("AAA", "Stock AAA", StockType.Ordinary);
            _AAAId = aaa.Id;

            var bbb = stockManager.Add("BBB", "Stock BBB", StockType.Ordinary);
            _BBBId = bbb.Id;

            var ccc = stockManager.Add("CCC", "Trust CCC", StockType.Trust);
            _CCCId = ccc.Id;

            var sss = stockManager.Add("SSS", "Stapled Security SSS", StockType.StapledSecurity);
            _SSSId = sss.Id;

            var sss1 = stockManager.Add("SSS1", "Stapled stock 1", StockType.Ordinary, sss);
            _SSS1Id = sss1.Id;

            var sss2 = stockManager.Add("SSS2", "Stapled stock 2", StockType.Ordinary, sss);
            _SSS2Id = sss2.Id;

            var sss3 = stockManager.Add("SSS3", "Stapled trust 3", StockType.Trust, sss);
            _SSS3Id = sss3.Id;

            sss1.AddRelativeNTA(new DateTime(2000, 01, 01), 0.10m);
            sss2.AddRelativeNTA(new DateTime(2000, 01, 01), 0.30m);
            sss3.AddRelativeNTA(new DateTime(2000, 01, 01), 0.60m);

            sss1.AddRelativeNTA(new DateTime(2001, 01, 01), 0.15m);
            sss2.AddRelativeNTA(new DateTime(2001, 01, 01), 0.35m);
            sss3.AddRelativeNTA(new DateTime(2001, 01, 01), 0.50m);

            sss1.AddRelativeNTA(new DateTime(2002, 01, 01), 0.20m);
            sss2.AddRelativeNTA(new DateTime(2002, 01, 01), 0.40m);
            sss3.AddRelativeNTA(new DateTime(2002, 01, 01), 0.40m);

            sss1.AddRelativeNTA(new DateTime(2003, 01, 01), 0.25m);
            sss2.AddRelativeNTA(new DateTime(2003, 01, 01), 0.45m);
            sss3.AddRelativeNTA(new DateTime(2003, 01, 01), 0.30m);

            sss1.AddRelativeNTA(new DateTime(2004, 01, 01), 0.30m);
            sss2.AddRelativeNTA(new DateTime(2004, 01, 01), 0.50m);
            sss3.AddRelativeNTA(new DateTime(2004, 01, 01), 0.20m);

            sss1.AddRelativeNTA(new DateTime(2005, 01, 01), 0.35m);
            sss2.AddRelativeNTA(new DateTime(2005, 01, 01), 0.55m);
            sss3.AddRelativeNTA(new DateTime(2005, 01, 01), 0.10m);

            sss1.AddRelativeNTA(new DateTime(2006, 01, 01), 0.40m);
            sss2.AddRelativeNTA(new DateTime(2006, 01, 01), 0.40m);
            sss3.AddRelativeNTA(new DateTime(2006, 01, 01), 0.20m);

            sss1.AddRelativeNTA(new DateTime(2007, 01, 01), 0.50m);
            sss2.AddRelativeNTA(new DateTime(2007, 01, 01), 0.20m);
            sss3.AddRelativeNTA(new DateTime(2007, 01, 01), 0.30m);

            sss1.AddRelativeNTA(new DateTime(2008, 01, 01), 0.60m);
            sss2.AddRelativeNTA(new DateTime(2008, 01, 01), 0.05m);
            sss3.AddRelativeNTA(new DateTime(2008, 01, 01), 0.35m);
        }

    }
}
