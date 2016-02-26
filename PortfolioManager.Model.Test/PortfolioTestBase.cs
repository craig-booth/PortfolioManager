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
using StockManager.Service;

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
            _StockDatabase = new SQLiteStockDatabase(":memory:");
            var stockServiceRepository = new StockServiceRepository(_StockDatabase);
            AddTestStocks(stockServiceRepository);
        }


        public StockServiceRepository CreateStockServiceRepository()
        {
            var stockDatabase = new SQLiteStockDatabase(":memory:");

            return new StockServiceRepository(stockDatabase);
        }

        public PortfolioManager.Model.Portfolios.PortfolioManager CreatePortfolioManager()
        {
            var portfolioDatabase = new SQLitePortfolioDatabase(":memory:");

            return new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);
        }

        public Portfolio CreateTestPortfolio()
        {
            var portfolioDatabase = new SQLitePortfolioDatabase(":memory:");
            var portfolioManager = new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);

            return portfolioManager.CreatePortfolio("Test portfolio");
        }

        public void AddTestStocks(StockServiceRepository stockServiceRepository)
        {
            var aaa = stockServiceRepository.StockService.Add("AAA", "Stock AAA", StockType.Ordinary);
            _AAAId = aaa.Id;

            var bbb = stockServiceRepository.StockService.Add("BBB", "Stock BBB", StockType.Ordinary);
            _BBBId = bbb.Id;

            var ccc = stockServiceRepository.StockService.Add("CCC", "Trust CCC", StockType.Trust);
            _CCCId = ccc.Id;

            var sss = stockServiceRepository.StockService.Add("SSS", "Stapled Security SSS", StockType.StapledSecurity);
            _SSSId = sss.Id;

            var sss1 = stockServiceRepository.StockService.Add("SSS1", "Stapled stock 1", StockType.Ordinary, sss);
            _SSS1Id = sss1.Id;

            var sss2 = stockServiceRepository.StockService.Add("SSS2", "Stapled stock 2", StockType.Ordinary, sss);
            _SSS2Id = sss2.Id;

            var sss3 = stockServiceRepository.StockService.Add("SSS3", "Stapled trust 3", StockType.Trust, sss);
            _SSS3Id = sss3.Id;

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2000, 01, 01), 0.10m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2000, 01, 01), 0.30m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2000, 01, 01), 0.60m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2001, 01, 01), 0.15m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2001, 01, 01), 0.35m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2001, 01, 01), 0.50m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2002, 01, 01), 0.20m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2002, 01, 01), 0.40m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2002, 01, 01), 0.40m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2003, 01, 01), 0.25m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2003, 01, 01), 0.45m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2003, 01, 01), 0.30m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2004, 01, 01), 0.30m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2004, 01, 01), 0.50m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2004, 01, 01), 0.20m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2005, 01, 01), 0.35m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2005, 01, 01), 0.55m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2005, 01, 01), 0.10m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2006, 01, 01), 0.40m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2006, 01, 01), 0.40m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2006, 01, 01), 0.20m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2007, 01, 01), 0.50m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2007, 01, 01), 0.20m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2007, 01, 01), 0.30m);

            stockServiceRepository.StockService.AddRelativeNTA(sss1,  new DateTime(2008, 01, 01), 0.60m);
            stockServiceRepository.StockService.AddRelativeNTA(sss2,  new DateTime(2008, 01, 01), 0.05m);
            stockServiceRepository.StockService.AddRelativeNTA(sss3,  new DateTime(2008, 01, 01), 0.35m);
        }

    }
}
