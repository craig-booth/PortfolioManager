using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios.Transactions
{
    [TestFixture]
    public abstract class TransactionTest
    {
        protected SQLiteStockDatabase _StockDatabase;
        protected StockManager _StockManager;
        protected Portfolio _Portfolio;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            _StockDatabase = new SQLiteStockDatabase("Data Source=:memory:;Version=3;");
            _StockManager = new StockManager(_StockDatabase);
            AddStocks();
        }

        [SetUp]
        public virtual void Setup()
        {
            var portfolioDatabase = new SQLitePortfolioDatabase("Data Source=:memory:;Version=3;");
            var portfolioManager = new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);

            _Portfolio = portfolioManager.CreatePortfolio("Test portfolio");
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

    [TestFixture]
    public abstract class TransactionTestWithExpectedTests : TransactionTest
    {
        protected DateTime _TransactionDate;

        protected List<ShareParcel> _ExpectedParcels;
        protected List<IncomeReceived> _ExpectedIncome;
        protected List<CGTEvent> _ExpectedCGTEvents;

        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _ExpectedParcels = new List<ShareParcel>();
            _ExpectedIncome = new List<IncomeReceived>();
            _ExpectedCGTEvents = new List<CGTEvent>();

            var portfolioDatabase = new SQLitePortfolioDatabase("Data Source=:memory:;Version=3;");
            var portfolioManager = new PortfolioManager.Model.Portfolios.PortfolioManager(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);

            _Portfolio = portfolioManager.CreatePortfolio("Test portfolio");
        }


        public override void Setup()
        {
            // Prevent base setup from running
        }

        [TestFixtureSetUp]
        public abstract void PerformTest();

        [Test]
        public void ExpectedParcels()
        {
            var actualParcels = _Portfolio.GetAllParcels(_TransactionDate);

            Assert.That(actualParcels, PortfolioConstraint.Equals(_ExpectedParcels));
        }

        [Test]
        public void ExpectedIncome()
        {
            var actualIncome = _Portfolio.GetIncomeReceived(DateTimeConstants.NoStartDate(), DateTimeConstants.NoEndDate());

            Assert.That(actualIncome, PortfolioConstraint.Equals(_ExpectedIncome));
        }

        [Test]
        public void ExpectedCGTEvents()
        {
            var actualCGTEvents = _Portfolio.GetCGTEvents(DateTimeConstants.NoStartDate(), DateTimeConstants.NoEndDate());

            Assert.That(actualCGTEvents, PortfolioConstraint.Equals(_ExpectedCGTEvents));
        }
    }
}
