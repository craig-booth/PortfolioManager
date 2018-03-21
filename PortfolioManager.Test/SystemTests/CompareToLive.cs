using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using AutoMapper;
using NUnit.Framework;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Local;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Test.SystemTests
{
    [TestFixture]
    public class CompareToLive
    {
        private IPortfolioDatabase _PortfolioDatabase;
        private IStockDatabase _StockDatabase;
        private IMapper _Mapper;

        private string _ExpectedResultsPath;
        private string _ActualResultsPath;

        [OneTimeSetUp]
        public async Task Init()
        {
            _ExpectedResultsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "ExpectedResults");
            _ActualResultsPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "SystemTests", "ActualResults");
            if (Directory.Exists(_ActualResultsPath))
            {
                var files = Directory.EnumerateFiles(_ActualResultsPath);
                foreach (var file in files)
                    File.Delete(file);
            }
            else
            {
                Directory.CreateDirectory(_ActualResultsPath);
            }

            _PortfolioDatabase = new SQLitePortfolioDatabase(Path.Combine(_ActualResultsPath, "Portfolio.db"));
            _StockDatabase = new SQLiteStockDatabase(Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "Stocks.db"));

            var config = new MapperConfiguration(cfg => 
                cfg.AddProfile(new ModelToServiceMapping(_StockDatabase))
            );
            _Mapper = config.CreateMapper();

            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
            var service = new TransactionService(_PortfolioDatabase, _StockDatabase, _Mapper);

            await service.ImportTransactions(Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "Transactions.xml"));         
        }

        private void SaveActualResult(ServiceResponce actual, string fileName)
        {
            var actualFile = Path.Combine(_ActualResultsPath, fileName);

            using (var streamWriter = new StreamWriter(actualFile))
            {
                var serializer = new XmlSerializer(actual.GetType());   

                serializer.Serialize(streamWriter, actual);
            }
        }

        private void SaveActualResult(ServiceResponce actual, string fileName, Type[] extraTypes)
        {
            var actualFile = Path.Combine(_ActualResultsPath, fileName);

            using (var streamWriter = new StreamWriter(actualFile))
            {
                var serializer = new XmlSerializer(actual.GetType(), extraTypes);

                serializer.Serialize(streamWriter, actual);
            }
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareCapitalGain(DateTime date)
        {
            var fileName = String.Format("CapitalGain {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new CapitalGainService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetDetailedUnrealisedGains(date);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(DetailedUnrealisedGainsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CGTLiability {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new CapitalGainService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetCGTLiability(fromDate, toDate);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(CGTLiabilityResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareCashTransactions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CashTransactions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new CashAccountService(_PortfolioDatabase);
            var responce = await service.GetTranasctions(fromDate, toDate);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(CashAccountTransactionsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareHoldings(DateTime date)
        {
            var fileName = String.Format("Holdings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new HoldingService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetHoldings(date);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(HoldingsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareTradeableHoldings(DateTime date)
        {
            var fileName = String.Format("TradeableHoldings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new HoldingService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetTradeableHoldings(date);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(HoldingsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareIncome(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("Income {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new IncomeService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetIncome(fromDate, toDate);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(IncomeResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task ComparePortfolioPerformance(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioPerformance {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new PortfolioPerformanceService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetPerformance(fromDate, toDate);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(PortfolioPerformanceResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task ComparePortfolioSummary(DateTime date)
        {
            var fileName = String.Format("PortfolioSummary {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new PortfolioSummaryService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetSummary(date);
            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(PortfolioSummaryResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task ComparePortfolioValue(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioValue {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new PortfolioValueService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetPortfolioValue(fromDate, toDate, ValueFrequency.Daily);

            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(PortfolioValueResponce), expectedFile));
        }

        [Test]
        public async Task CompareUnappliedCorporateActions()
        {
            var fileName = "UnappliedCorporateActions.xml";
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var service = new CorporateActionService(_PortfolioDatabase, _StockDatabase, _Mapper);
            var responce = await service.GetUnappliedCorporateActions();

            SaveActualResult(responce, fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(PortfolioValueResponce), expectedFile));
        }
    }

    public class CompareToLiveTestData
    {
        public static IEnumerable TestDates
        {
            get
            {
                var date = new DateTime(2016, 01, 01);
                while (date.Year <= 2017)
                {
                    var testData = new TestCaseData(date).SetName("{m} (" + date.ToString("yyyy-MM-dd") + ")");       
                    date = date.AddMonths(1);

                    yield return testData;
                }
            }
        }

        public static IEnumerable TestDateRanges
        {
            get
            {
                var date = new DateTime(2016, 01, 01);
                while (date.Year <= 2017)
                {
                    var fromDate = date.AddYears(-1).AddDays(1);
                    var testData = new TestCaseData(fromDate, date).SetName("{m} (" + fromDate.ToString("yyyy-MM-dd") + " to " + date.ToString("yyyy-MM-dd") + ")");
                    date = date.AddMonths(1);

                    yield return testData;
                }
            }
        }
    }
}
