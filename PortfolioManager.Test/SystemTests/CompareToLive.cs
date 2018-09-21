using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using NUnit.Framework;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Services;
using PortfolioManager.Service.Utils;
using PortfolioManager.Web.Controllers.v1;
using PortfolioManager.Web.Controllers.v2;

namespace PortfolioManager.Test.SystemTests
{
    [TestFixture]
    public class CompareToLive
    {
        private IPortfolioDatabase _PortfolioDatabase;
        private StockExchange _StockExchange;
        private IMapper _Mapper;
        private ServiceProvider _ServiceProvider; 

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

            var eventStore = new MongodbEventStore("mongodb://192.168.99.100:27017");
            _StockExchange = new StockExchange(eventStore);
            _StockExchange.LoadFromEventStream();

            _PortfolioDatabase = new SQLitePortfolioDatabase(Path.Combine(_ActualResultsPath, "Portfolio.db"));

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ModelToServiceMapping(_StockExchange))
            );
            _Mapper = config.CreateMapper();


            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IPortfolioDatabase>(_PortfolioDatabase);
            services.AddSingleton<StockExchange>(_StockExchange);
            services.AddSingleton<IMapper>(_Mapper);
            services.AddScoped<IPortfolioSummaryService, PortfolioSummaryService>();
            services.AddScoped<IPortfolioPerformanceService, PortfolioPerformanceService>();
            services.AddScoped<ICapitalGainService, CapitalGainService>();
            services.AddScoped<IPortfolioValueService, PortfolioValueService>();
            services.AddScoped<ICorporateActionService, CorporateActionService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IHoldingService, HoldingService>();
            services.AddScoped<ICashAccountService, CashAccountService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IStockService, StockService>();

            _ServiceProvider = services.BuildServiceProvider();

            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
            var service = new TransactionService(_PortfolioDatabase, _StockExchange, _Mapper);

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

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetDetailedCapitalGains(null, date);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(DetailedUnrealisedGainsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CGTLiability {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetCGTLiability(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(CGTLiabilityResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareCashTransactions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CashTransactions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetCashAccountTransactions(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(CashAccountTransactionsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareHoldings(DateTime date)
        {
            var fileName = String.Format("Holdings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);    
            var response = await controller.GetHoldings(date, false);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(HoldingsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareTradeableHoldings(DateTime date)
        {
            var fileName = String.Format("TradeableHoldings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetHoldings(date, true);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(HoldingsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareIncome(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("Income {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetIncome(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(IncomeResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task ComparePortfolioPerformance(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioPerformance {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetPerformance(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(PortfolioPerformanceResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task ComparePortfolioSummary(DateTime date)
        {
            var fileName = String.Format("PortfolioSummary {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetSummary(date);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(PortfolioSummaryResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task ComparePortfolioValue(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioValue {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetPortfolioValue(null, fromDate, toDate, ValueFrequency.Daily);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(PortfolioValueResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareCorporateActions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CorporateActions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            CorporateActionsResponce response = new CorporateActionsResponce();

            var controller = new CorporateActionController(_StockExchange.Stocks);

            var stocks = _StockExchange.Stocks.All().Where(x => x.IsEffectiveDuring(new DateRange(fromDate, toDate))).OrderBy(x => x, new StockComparer());
            foreach (var stock in stocks)
            {
                var stockItem = new StockItem(stock.Id, stock.Properties.ClosestTo(toDate).ASXCode, stock.Properties.ClosestTo(toDate).Name);

                var result = controller.GetCorporateActions(stock.Id, fromDate, toDate);

                if (result.Value != null)
                {
                    var corporateActions = result.Value.Select(x => new CorporateActionItem()
                    {
                        Id = x.Id,
                        ActionDate = x.ActionDate,
                        Stock = stockItem,
                        Description = x.Description
                    });

                    response.CorporateActions.AddRange(corporateActions);
                }

            }

            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(CorporateActionsResponce), expectedFile));
        }

        [Test]
        public async Task CompareUnappliedCorporateActions()
        {
            var fileName = "UnappliedCorporateActions.xml";
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetUnappliedCorporateActions();
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(UnappliedCorporateActionsResponce), expectedFile));
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

    class StockComparer : IComparer<Domain.Stocks.Stock>
    {
        static List<string> Codes = new List<string>() { "ARG", "AMP", "NAB", "TLS", "AGI", "TWE", "CSL" , "BHP", "COH", "WAM", "GVF", "S32", "VGS", "VAF", "VAP", "VCF"};

        public int Compare(Domain.Stocks.Stock stock1, Domain.Stocks.Stock stock2)
        {
            var code1 = stock1.Properties.ClosestTo(DateTime.Today).ASXCode;
            var code2 = stock2.Properties.ClosestTo(DateTime.Today).ASXCode;

            var i1 = Codes.IndexOf(code1);
            var i2 = Codes.IndexOf(code2);

            if ((i1 == -1) && (i2 == -1))
                return code1.CompareTo(code2);
            else if (i1 == -1)
                return 1;
            else if (i2 == -1)
                return -1;
            else
                return i1.CompareTo(i2);
        }
    }
}
