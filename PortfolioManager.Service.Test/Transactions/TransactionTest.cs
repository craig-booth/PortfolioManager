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

namespace PortfolioManager.Service.Test.Transactions
{

    [TestFixture, Description("General transaction tests")]
    public class GeneralTransactionTests : PortfolioTest
    {
        [Test, Description("Multiple transactions on the same date")]
        public void MultipleTransactionsOnTheSameDay()
        {
            var transactionDate = new DateTime(2010, 01, 01);
            var transactions = new Transaction[]
            {
                new Aquisition()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    RecordDate = transactionDate,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    RecordDate = transactionDate,
                    Amount = 0.40m,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    RecordDate = transactionDate,
                    Amount = 0.41m,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    RecordDate = transactionDate,
                    Amount = 0.42m,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    RecordDate = transactionDate,
                    Amount = 0.43m,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    RecordDate = transactionDate,
                    Amount = 0.44m,
                    Comment = ""
                },
                new Disposal
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    RecordDate = transactionDate,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // Check that transactions are stored in the correct order
            var actualTransactions = _Portfolio.TransactionService.GetTransactions(transactionDate, transactionDate);

            var expectedTransactionIds = transactions.Select<Transaction, Guid>(x => x.Id).ToList();
            var actualTransactionIds = actualTransactions.Select<Transaction, Guid>(x => x.Id).ToList();

            Assert.That(actualTransactionIds, Is.EqualTo(expectedTransactionIds));
        }
    }

    [TestFixture]
    public abstract class TransactionTestWithExpectedTests : PortfolioTest
    {
        protected DateTime _TransactionDate;

        protected List<ShareParcel> _ExpectedParcels;
        protected List<Income> _ExpectedIncome;
        protected List<CGTEvent> _ExpectedCGTEvents;

        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _ExpectedParcels = new List<ShareParcel>();
            _ExpectedIncome = new List<Income>();
            _ExpectedCGTEvents = new List<CGTEvent>();

            var portfolioDatabase = new SQLitePortfolioDatabase(":memory:");
            //var portfolioDatabase = new SQLitePortfolioDatabase("C:\\Users\\CraigB\\Desktop\\test.db");
            _Portfolio = new Portfolio(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);
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
            var actualParcels = _Portfolio.ParcelService.GetParcels(_TransactionDate);

            Assert.That(actualParcels, EntityConstraint.CollectionEquivalant(_ExpectedParcels));
        }

        [Test]
        public void ExpectedIncome()
        {
            var actualIncome = _Portfolio.IncomeService.GetIncome(DateUtils.NoStartDate, DateUtils.NoEndDate);

            Assert.That(actualIncome, EntityConstraint.CollectionEquivalant(_ExpectedIncome));
        }

        [Test]
        public void ExpectedCGTEvents()
        {
            var actualCGTEvents = _Portfolio.CGTService.GetEvents(DateUtils.NoStartDate, DateUtils.NoEndDate);

            Assert.That(actualCGTEvents, EntityConstraint.CollectionEquivalant(_ExpectedCGTEvents));
        }

        public Stock GetStock(string asxCode)
        {
            return _StockServiceRepository.StockService.GetStock(asxCode);
        }

        public Guid GetStockId(string asxCode)
        {
            return GetStock(asxCode).Id;
        }

        public Income IncomeFromTransacation(IncomeReceived transaction)
        {
            return new Income(GetStock(transaction.ASXCode), transaction.FrankedAmount, transaction.UnfrankedAmount, transaction.FrankingCredits, transaction.Interest, transaction.TaxDeferred);
        }


    }
}
