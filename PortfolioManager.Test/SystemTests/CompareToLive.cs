using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;

using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.Converters;

using PortfolioManager.Web;
using PortfolioManager.Web.Controllers.v2;


namespace PortfolioManager.Test.SystemTests
{
    [TestFixture]
    public class CompareToLive
    {
        private Guid _PortfolioId = Guid.NewGuid();
        private ServiceProvider _ServiceProvider; 

        private string _ExpectedResultsPath;
        private string _ActualResultsPath;

        [OneTimeSetUp]
        public void Init()
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

            var settings = new PortfolioManagerSettings()
            {
                ApiKey = Guid.Empty,
                //EventStore = "mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017",
                EventStore = "mongodb://192.168.99.100:27017",
                Port = 0
            };
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddPortfolioManagerService(settings);

            services.AddSingleton<TransactionController>();
            services.AddSingleton<HoldingController>();
            services.AddSingleton<CorporateActionController>();
            services.AddSingleton<PortfolioController>();

            _ServiceProvider = services.BuildServiceProvider();
            _ServiceProvider.InitializeStockExchange();

            var portfolio = new PortfolioManager.Domain.Portfolios.Portfolio();
            portfolio.Create(_PortfolioId, "Compare To Live");
            var cache = _ServiceProvider.GetRequiredService<PortfolioManager.Domain.IEntityCache<PortfolioManager.Domain.Portfolios.Portfolio>>();
            cache.Add(portfolio);

            LoadTransactions();
        }

        public void LoadTransactions()
        {
            var transactionConverter = _ServiceProvider.GetRequiredService<TransactionJsonConverter>();

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            settings.Converters.Add(transactionConverter);
            settings.Converters.Add(new CorporateActionJsonConverter());
            var serializer = JsonSerializer.Create(settings);

            var jsonFile = File.OpenText(Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "Transactions.json"));
            var jsonReader = new JsonTextReader(jsonFile);
            var transactions = serializer.Deserialize<List<RestApi.Transactions.Transaction>>(jsonReader);
            jsonFile.Close();

            var controller = _ServiceProvider.GetRequiredService<TransactionController>(); 
            SetControllerContext(controller);

            controller.AddTransactions(transactions.Where(x => x != null).ToList());
        }

        private void SaveActualResult(object actual, string fileName)
        {
            var actualFile = Path.Combine(_ActualResultsPath, fileName);

            using (var streamWriter = new StreamWriter(actualFile))
            {
                var serializer = new XmlSerializer(actual.GetType());   

                serializer.Serialize(streamWriter, actual);
            }
        }

        private void SaveActualResult(object actual, string fileName, Type[] extraTypes)
        {
            var actualFile = Path.Combine(_ActualResultsPath, fileName);

            using (var streamWriter = new StreamWriter(actualFile))
            {
                var serializer = new XmlSerializer(actual.GetType(), extraTypes);

                serializer.Serialize(streamWriter, actual);
            }
        }

        private void SetControllerContext(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            routeData.Values["portfolioId"] = _PortfolioId.ToString();
            var actionDescription = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var context = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller);

            controller.OnActionExecuting(context);
        } 

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareTransactions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("Transactions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetTransactions(fromDate, toDate);
            SaveActualResult(response.Value, fileName, new Type[]
                        {
                            typeof(RestApi.Transactions.Aquisition),
                            typeof(RestApi.Transactions.CashTransaction),
                            typeof(RestApi.Transactions.CostBaseAdjustment),
                            typeof(RestApi.Transactions.Disposal),
                            typeof(RestApi.Transactions.IncomeReceived),
                            typeof(RestApi.Transactions.OpeningBalance),
                            typeof(RestApi.Transactions.ReturnOfCapital),
                            typeof(RestApi.Transactions.UnitCountAdjustment)
                        });


            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.TransactionsResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public void CompareCapitalGain(DateTime date)
        {
            var fileName = String.Format("CapitalGain {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetDetailedCapitalGains(date);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.DetailedUnrealisedGainsResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CGTLiability {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetCGTLiability(fromDate, toDate);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.CgtLiabilityResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareCashTransactions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CashTransactions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetCashAccountTransactions(fromDate, toDate);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.CashAccountTransactionsResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public void CompareHoldings(DateTime date)
        {
            var fileName = String.Format("Holdings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<HoldingController>();
            SetControllerContext(controller);

            var response = controller.Get(null, date);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(List<RestApi.Portfolios.Holding>), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareIncome(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("Income {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetIncome(fromDate, toDate);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.IncomeResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void ComparePortfolioPerformance(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioPerformance {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetPerformance(fromDate, toDate);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.PortfolioPerformanceResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public void ComparePortfolioSummary(DateTime date)
        {
            var fileName = String.Format("PortfolioSummary {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetSummary(date);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.PortfolioSummaryResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void ComparePortfolioValue(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioValue {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetValue(fromDate, toDate, RestApi.Portfolios.ValueFrequency.Daily);
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.PortfolioValueResponse), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareCorporateActions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CorporateActions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var response = new List<RestApi.CorporateActions.CorporateAction>();

            var controller = _ServiceProvider.GetRequiredService<CorporateActionController>();

            var stockQuery = _ServiceProvider.GetRequiredService<IStockQuery>();
            var stocks = stockQuery.All(new DateRange(fromDate, toDate)).OrderBy(x => x, new StockComparer());
            foreach (var stock in stocks)
            {
                var corporateActions = controller.GetCorporateActions(stock.Id, fromDate, toDate);

                response.AddRange(corporateActions.Value);
            }

            var actionTypes = new Type[] 
            {
                typeof(RestApi.CorporateActions.CapitalReturn),
                typeof(RestApi.CorporateActions.CompositeAction),
                typeof(RestApi.CorporateActions.Dividend),
                typeof(RestApi.CorporateActions.SplitConsolidation),
                typeof(RestApi.CorporateActions.Transformation)
            };
            SaveActualResult(response, fileName, actionTypes);

            Assert.That(response, Is.EquivalentTo(typeof(List<RestApi.CorporateActions.CorporateAction>), expectedFile, actionTypes));
        }

        [Test]
        public void ComparePortfolioProperties()
        {
            var fileName = "PortfolioProperties.xml";
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetProperties();
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.PortfolioPropertiesResponse), expectedFile));
        }

        [Test]
        public void CompareUnappliedCorporateActions()
        {
            var fileName = "UnappliedCorporateActions.xml";
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetCorporateActions();
            SaveActualResult(response.Value, fileName);

            Assert.That(response.Value, Is.EquivalentTo(typeof(RestApi.Portfolios.CorporateActionsResponse), expectedFile));
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
