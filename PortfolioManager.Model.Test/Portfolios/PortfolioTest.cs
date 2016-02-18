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

namespace PortfolioManager.Model.Test.Portfolios
{
    [TestFixture]
    public abstract class PortfolioTest
    {
        protected SQLiteStockDatabase _StockDatabase;
        protected StockManager _StockManager;
        protected PortfolioManager.Model.Portfolios.PortfolioManager _PortfolioManager;
        protected Portfolio _Portfolio;

        public PortfolioTest()
        {
            EntityConstraint.RegisterComparer(typeof(ShareParcel), "Id");
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
            _StockManager = new StockManager(_StockDatabase);
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
            _StockManager.Add("AAA", "Stock AAA", StockType.Ordinary);
            _StockManager.Add("BBB", "Stock BBB", StockType.Ordinary);
            _StockManager.Add("CCC", "Trust CCC", StockType.Trust);

            var sss = _StockManager.Add("SSS", "Stapled Security SSS", StockType.StapledSecurity);
            var sss1 = _StockManager.Add("SSS1", "Stapled stock 1", StockType.Ordinary, sss);
            var sss2 = _StockManager.Add("SSS2", "Stapled stock 2", StockType.Ordinary, sss);
            var sss3 = _StockManager.Add("SSS3", "Stapled trust 3", StockType.Trust, sss);

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
