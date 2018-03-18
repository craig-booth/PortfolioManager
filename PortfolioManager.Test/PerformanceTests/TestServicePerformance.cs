using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using NUnit.Framework;
using NBench;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.EventStore;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Services;

namespace PortfolioManager.Test.PerformanceTests
{
    class TestServicePerformance : PerformanceTestStuite<TestServicePerformance>
    {
        private IPortfolioDatabase _PortfolioDatabase;
        private IStockDatabase _StockDatabase;
        private StockExchange _StockExchange;

        private string _TestPath;

        private Counter _Counter;

        private DateTime _AtDate;
        private DateTime _FromDate;
        private DateTime _ToDate;

        [PerfSetup]
        public async void Init(BenchmarkContext context)
        {
            _TestPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PerformanceTests");

            _AtDate = new DateTime(2017, 06, 30);
            _FromDate = new DateTime(2016, 07, 01);
            _ToDate = new DateTime(2017, 06, 30);

            var eventStore = new SqliteEventStore(Path.Combine(_TestPath, "Events.db"));
            _StockExchange = new StockExchange(eventStore);
            _StockExchange.Load();

            var portfolioDatabaseFile = Path.Combine(_TestPath, "Portfolio.db");
            File.Delete(portfolioDatabaseFile);
            _PortfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabaseFile);

            var stockDatabaseFile = Path.Combine(_TestPath, "Stocks.db");
            _StockDatabase = new SQLiteStockDatabase(stockDatabaseFile);

            await LoadTransactions();

            _Counter = context.GetCounter("TestCounter");
        }

        public async Task LoadTransactions()
        {
            var service = new TransactionService(_PortfolioDatabase, _StockExchange);

            await service.ImportTransactions(Path.Combine(_TestPath, "Transactions.xml"));
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestCapitalGainServicePerformance()
        {
            var service = new CapitalGainService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetDetailedUnrealisedGains(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestCGTLiabilityServicePerformance()
        {
            var service = new CapitalGainService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetCGTLiability(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestCashTransactionsServicePerformance()
        {
            var service = new CashAccountService(_PortfolioDatabase);
            var responce = await service.GetTranasctions(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestHoldingsServicePerformance()
        {
            var service = new HoldingService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetHoldings(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestTradeableHoldingsServicePerformance()
        {
            var service = new HoldingService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetTradeableHoldings(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestIncomeServicePerformance()
        {
            var service = new IncomeService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetIncome(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestPortfolioPerformanceServicePerformance()
        {
            var service = new PortfolioPerformanceService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetPerformance(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestPortfolioSummaryServicePerformance()
        {
            var service = new PortfolioSummaryService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetSummary(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestPortfolioValueServicePerformance()
        {
            var service = new PortfolioValueService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetPortfolioValue(_FromDate, _ToDate, ValueFrequency.Daily);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10000.0d)]
        public async void TestTransactionsServicePerformance()
        {
            var service = new TransactionService(_PortfolioDatabase, _StockExchange);
            var responce = await service.GetTransactions(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public async void TestStockServicePerformance()
        {
            using (var unitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
            {
                var service = new StockService(_StockExchange, unitOfWork.CorporateActionQuery);
                var responce = await service.GetStocks(_AtDate, true, true);
            }

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
            NumberOfIterations = 3, RunMode = RunMode.Throughput,
            RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 1000.0d)]
        public async void TestCorporateActionsForStockServicePerformance()
        {
            
            using (var unitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
            {
                var service = new StockService(_StockExchange, unitOfWork.CorporateActionQuery);

                var stocks = unitOfWork.StockQuery.GetAll();
                foreach (var stock in stocks.Where(x => x.IsWithinRange(_FromDate, _ToDate)))
                {
                        var responce = await service.GetCorporateActions(stock.Id, _FromDate, _ToDate);
                }
            }

            _Counter.Increment();
        }
    }
}
