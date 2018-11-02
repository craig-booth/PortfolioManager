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

using AutoMapper;
using NUnit.Framework;
using NBench;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Services;
using PortfolioManager.Service.Utils;

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
                var portfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabaseFile);

                var eventStore = new MongodbEventStore(eventStorePath);
                var stockExchange = new StockExchange(eventStore);
                stockExchange.LoadFromEventStream();

                var config = new MapperConfiguration(cfg =>
                    cfg.AddProfile(new ModelToServiceMapping(stockExchange))
                );
                var mapper = config.CreateMapper();

                ServiceCollection services = new ServiceCollection();
                services.AddLogging();

                services.AddSingleton<IPortfolioDatabase>(portfolioDatabase);
                services.AddSingleton<StockExchange>(stockExchange);
                services.AddSingleton<IStockRepository>(stockExchange.Stocks);
                services.AddSingleton<IMapper>(mapper);
                services.AddScoped<IPortfolioSummaryService, PortfolioSummaryService>();
                services.AddScoped<IPortfolioPerformanceService, PortfolioPerformanceService>();
                services.AddScoped<ICapitalGainService, CapitalGainService>();
                services.AddScoped<IPortfolioValueService, PortfolioValueService>();
                services.AddScoped<ICorporateActionService, CorporateActionService>();
                services.AddScoped<IHoldingService, HoldingService>();
                services.AddScoped<ICashAccountService, CashAccountService>();
                services.AddScoped<IIncomeService, IncomeService>();
                services.AddScoped<ITransactionService, TransactionService>();

                _ServiceProvider = services.BuildServiceProvider();

                // Load transactions into Portfolio Database
                var service = _ServiceProvider.GetRequiredService<ITransactionService>();
                var importTask = service.ImportTransactions(Path.Combine(testPath, "Transactions.xml"));
                importTask.Wait();
            }

            return _ServiceProvider;
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
            //  var eventStorePath = "mongodb://192.168.99.100:27017";
            var eventStorePath = "mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017";

            _ServiceProvider = TestServicePerformanceEnvironment.GetServiceProvider(testPath, eventStorePath);

            _AtDate = new DateTime(2017, 06, 30);
            _FromDate = new DateTime(2016, 07, 01);
            _ToDate = new DateTime(2017, 06, 30);

            _Counter = context.GetCounter("TestCounter");
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestCapitalGainServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetDetailedCapitalGains(null, _AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestCGTLiabilityServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetCGTLiability(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestCashTransactionsServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetCashAccountTransactions(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestHoldingsServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetHoldings(_AtDate, false);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestTradeableHoldingsServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetHoldings(_AtDate, true);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestIncomeServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetIncome(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestPortfolioPerformanceServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetPerformance(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestPortfolioSummaryServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetSummary(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestPortfolioValueServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = await controller.GetPortfolioValue(null, _FromDate, _ToDate, ValueFrequency.Daily);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestTransactionsServicePerformance()
        {
            var controller = new Web.Controllers.v1.TransactionController(_ServiceProvider);
            var response = await controller.Get(null, _FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public void TestStockServicePerformance()
        {
            var stockRepository = _ServiceProvider.GetRequiredService<IStockRepository>();
            var controller = new Web.Controllers.v1.StockController(stockRepository);
            var response = controller.Get(null, _AtDate, null, null);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public void TestCorporateActionsForStockServicePerformance()
        {
            var stockExchange = _ServiceProvider.GetRequiredService<StockExchange>();
            var stockRepository = _ServiceProvider.GetRequiredService<IStockRepository>();

            var controller = new Web.Controllers.v1.StockController(stockRepository);

            var service = new StockService(stockExchange);

            var stocks = stockRepository.All().Where(x => x.IsEffectiveDuring(new DateRange(_FromDate, _ToDate)));
            foreach (var stock in stocks)
            {
                //var responce = controller.co await service.GetCorporateActions(stock.Id, _FromDate, _ToDate);
            } 

            _Counter.Increment();
        }


        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public void TestUnappliedCorporateActionsServicePerformance()
        {
            var controller = new Web.Controllers.v1.PortfolioController(_ServiceProvider);
            var response = controller.GetUnappliedCorporateActions();

            _Counter.Increment();
        }
    }
}
