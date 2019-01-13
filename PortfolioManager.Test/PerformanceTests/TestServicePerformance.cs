using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;

using NBench;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Web;
using PortfolioManager.Web.Controllers.v2;
using PortfolioManager.Web.Converters;

namespace PortfolioManager.Test.PerformanceTests
{

    static class TestServicePerformanceEnvironment
    {
        private static ServiceProvider _ServiceProvider;
        public static ServiceProvider GetServiceProvider(string testPath, string eventStorePath)
        {
            if (_ServiceProvider == null)
            {
                var portfolioDatabaseFile = Path.Combine(testPath, "Portfolio.db");
                File.Delete(portfolioDatabaseFile);

                var settings = new PortfolioManagerSettings()
                {
                    ApiKey = Guid.Empty,
                    PortfolioDatabase = portfolioDatabaseFile,
                    EventStore = "mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017",
                    //EventStore = "mongodb://192.168.99.100:27017",
                    Port = 0
                };
                ServiceCollection services = new ServiceCollection();
                services.AddLogging();
                services.AddPortfolioManagerService(settings);

                services.AddSingleton<TransactionController>();
                services.AddSingleton<HoldingController>();
                services.AddSingleton<CorporateActionController>();
                services.AddSingleton<StockController>();
                services.AddSingleton<PortfolioController>();

                _ServiceProvider = services.BuildServiceProvider();

                var stockExchange = _ServiceProvider.GetRequiredService<StockExchange>();
                stockExchange.LoadFromEventStream();

                LoadTransactions(testPath);
            }

            return _ServiceProvider;
        }

        public static void LoadTransactions(string testPath)
        {
            var transactionConverter = _ServiceProvider.GetRequiredService<TransactionJsonConverter>();

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            settings.Converters.Add(transactionConverter);
            settings.Converters.Add(new CorporateActionJsonConverter());
            var serializer = JsonSerializer.Create(settings);

            var jsonFile = File.OpenText(Path.Combine(testPath, "Transactions.json"));
            var jsonReader = new JsonTextReader(jsonFile);
            var transactions = serializer.Deserialize<List<RestApi.Transactions.Transaction>>(jsonReader);
            jsonFile.Close();

            var controller = _ServiceProvider.GetRequiredService<TransactionController>();
            SetControllerContext(controller);

            controller.AddTransactions(transactions.Where(x => x != null).ToList());
        }

        private static void SetControllerContext(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            routeData.Values["portfolioId"] = Guid.NewGuid().ToString();
            var actionDescription = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var context = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller);

            controller.OnActionExecuting(context);
        }

    }

    class TestServicePerformance : PerformanceTestStuite<TestServicePerformance>
    {
        private Counter _Counter;

        private DateTime _AtDate;
        private DateTime _FromDate;
        private DateTime _ToDate;

        private ServiceProvider _ServiceProvider;

        [PerfSetup]
        public void Init(BenchmarkContext context)
        {
            var testPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PerformanceTests");
            //var eventStorePath = "mongodb://192.168.99.100:27017";
            var eventStorePath = "mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017";

            _ServiceProvider = TestServicePerformanceEnvironment.GetServiceProvider(testPath, eventStorePath);

            _AtDate = new DateTime(2017, 06, 30);
            _FromDate = new DateTime(2016, 07, 01);
            _ToDate = new DateTime(2017, 06, 30);

            _Counter = context.GetCounter("TestCounter");
        }

        private void SetControllerContext(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            routeData.Values["portfolioId"] = Guid.NewGuid().ToString();
            var actionDescription = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var context = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller);

            controller.OnActionExecuting(context);
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestCapitalGainServicePerformance()
        {   
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetDetailedCapitalGains(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestCGTLiabilityServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetCGTLiability(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestCashTransactionsServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetCashAccountTransactions(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestHoldingsServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<HoldingController>();
            SetControllerContext(controller);

            var response = controller.Get(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestIncomeServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetIncome(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestPortfolioPerformanceServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetPerformance(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestPortfolioSummaryServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetSummary(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestPortfolioValueServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetValue(_FromDate, _ToDate, RestApi.Portfolios.ValueFrequency.Daily);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public void TestTransactionsServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            var response = controller.GetTransactions(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public void TestStockServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<StockController>();
            SetControllerContext(controller);

            var response = controller.Get(null, _AtDate, null, null);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public void TestCorporateActionsForStockServicePerformance()
        {
            var stockRepository = _ServiceProvider.GetRequiredService<IStockRepository>();

            var controller = _ServiceProvider.GetRequiredService<CorporateActionController>();
            SetControllerContext(controller);


            var stocks = stockRepository.All().Where(x => x.IsEffectiveDuring(new DateRange(_FromDate, _ToDate)));
            foreach (var stock in stocks)
            {
                var response = controller.GetCorporateActions(stock.Id, _FromDate, _ToDate);
            }  

            _Counter.Increment();
        }


        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public void TestUnappliedCorporateActionsServicePerformance()
        {
            var controller = _ServiceProvider.GetRequiredService<PortfolioController>();
            SetControllerContext(controller);

            controller.GetCorporateActions();

            _Counter.Increment();
        }
    }
}
