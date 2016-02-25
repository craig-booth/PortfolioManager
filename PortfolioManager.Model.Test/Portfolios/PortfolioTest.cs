using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnitExtension;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using StockManager.Service;

namespace PortfolioManager.Model.Test.Portfolios
{
    [TestFixture]
    public abstract class PortfolioTest
    {
        protected SQLiteStockDatabase _StockDatabase;
        protected StockManager.Service.StockManager _StockManager;
        protected PortfolioManager.Model.Portfolios.PortfolioManager _PortfolioManager;
        protected Portfolio _Portfolio;

        public PortfolioTest()
        {
            EntityConstraint.RegisterComparer(typeof(ShareParcel), new ShareParcelComparer(), new EntityWriter("Id"));
            EntityConstraint.RegisterComparer(typeof(CGTEvent), "Id");
            EntityConstraint.RegisterComparer(typeof(Income), "Id");

            EntityConstraint.RegisterComparer(typeof(Aquisition), "Id");
            EntityConstraint.RegisterComparer(typeof(CostBaseAdjustment), "Id");
            EntityConstraint.RegisterComparer(typeof(Disposal), "Id");
            EntityConstraint.RegisterComparer(typeof(IncomeReceived), "Id");
            EntityConstraint.RegisterComparer(typeof(OpeningBalance), "Id");
            EntityConstraint.RegisterComparer(typeof(ReturnOfCapital), "Id");
            EntityConstraint.RegisterComparer(typeof(UnitCountAdjustment), "Id");
        }

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            _StockDatabase = new SQLiteStockDatabase(":memory:");
            _StockManager = new StockManager.Service.StockManager(_StockDatabase);
            AddStocks();
        }

        [SetUp]
        public virtual void Setup()
        {
            var portfolioDatabase = new SQLitePortfolioDatabase(":memory:");
            _PortfolioManager = new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);

            _Portfolio = _PortfolioManager.CreatePortfolio("Test portfolio");
        }

        protected virtual void AddStocks()
        {
            _StockManager.StockService.Add("AAA", "Stock AAA", StockType.Ordinary);
            _StockManager.StockService.Add("BBB", "Stock BBB", StockType.Ordinary);
            _StockManager.StockService.Add("CCC", "Trust CCC", StockType.Trust);

            var sss = _StockManager.StockService.Add("SSS", "Stapled Security SSS", StockType.StapledSecurity);
            var sss1 = _StockManager.StockService.Add("SSS1", "Stapled stock 1", StockType.Ordinary, sss);
            var sss2 = _StockManager.StockService.Add("SSS2", "Stapled stock 2", StockType.Ordinary, sss);
            var sss3 = _StockManager.StockService.Add("SSS3", "Stapled trust 3", StockType.Trust, sss);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2000, 01, 01), 0.10m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2000, 01, 01), 0.30m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2000, 01, 01), 0.60m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2001, 01, 01), 0.15m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2001, 01, 01), 0.35m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2001, 01, 01), 0.50m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2002, 01, 01), 0.20m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2002, 01, 01), 0.40m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2002, 01, 01), 0.40m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2003, 01, 01), 0.25m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2003, 01, 01), 0.45m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2003, 01, 01), 0.30m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2004, 01, 01), 0.30m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2004, 01, 01), 0.50m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2004, 01, 01), 0.20m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2005, 01, 01), 0.35m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2005, 01, 01), 0.55m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2005, 01, 01), 0.10m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2006, 01, 01), 0.40m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2006, 01, 01), 0.40m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2006, 01, 01), 0.20m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2007, 01, 01), 0.50m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2007, 01, 01), 0.20m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2007, 01, 01), 0.30m);

            _StockManager.StockService.AddRelativeNTA(sss1, new DateTime(2008, 01, 01), 0.60m);
            _StockManager.StockService.AddRelativeNTA(sss2, new DateTime(2008, 01, 01), 0.05m);
            _StockManager.StockService.AddRelativeNTA(sss3,  new DateTime(2008, 01, 01), 0.35m);

        }
    }

}
