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
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Local;

namespace PortfolioManager.Test.PerformanceTests
{
    class TestServicePerformance : PerformanceTestStuite<TestServicePerformance>
    {
        private bool _Initialized = false;
        private IPortfolioDatabase _PortfolioDatabase;
        private IStockDatabase _StockDatabase;

        private string _TestPath;

        private Counter _Counter;

        private DateTime _AtDate;
        private DateTime _FromDate;
        private DateTime _ToDate;

        [PerfSetup]
        public async void Init(BenchmarkContext context)
        {
            if (!_Initialized)
            {
                _TestPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "PerformanceTests");

                _AtDate = new DateTime(2017, 06, 30);
                _FromDate = new DateTime(2016, 07, 01);
                _ToDate = new DateTime(2017, 06, 30);

                var portfolioDatabaseFile = Path.Combine(_TestPath, "Portfolio.db");
                File.Delete(portfolioDatabaseFile);
                _PortfolioDatabase = new SQLitePortfolioDatabase(portfolioDatabaseFile);
                _StockDatabase = new SQLiteStockDatabase(Path.Combine(_TestPath, "Stocks.db"));

                await LoadTransactions();

                _Initialized = true;
            }

            _Counter = context.GetCounter("TestCounter");
        }

        public async Task LoadTransactions()
        {
            var service = new TransactionService(_PortfolioDatabase, _StockDatabase);

            await service.ImportTransactions(Path.Combine(_TestPath, "Transactions.xml"));
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
                NumberOfIterations = 3, RunMode = RunMode.Throughput,
                RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 60.0d)]
        public async void TestCapitalGainServicePerformance()
        {
            var service = new CapitalGainService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetDetailedUnrealisedGains(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 500.0d)]
        public async void TestCGTLiabilityServicePerformance()
        {
            var service = new CapitalGainService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetCGTLiability(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 700.0d)]
        public async void TestCashTransactionsServicePerformance()
        {
            var service = new CashAccountService(_PortfolioDatabase);
            var responce = await service.GetTranasctions(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 250.0d)]
        public async void TestHoldingsServicePerformance()
        {
            var service = new HoldingService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetHoldings(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 250.0d)]
        public async void TestTradeableHoldingsServicePerformance()
        {
            var service = new HoldingService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetTradeableHoldings(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 200.0d)]
        public async void TestIncomeServicePerformance()
        {
            var service = new IncomeService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetIncome(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 50.0d)]
        public async void TestPortfolioPerformanceServicePerformance()
        {
            var service = new PortfolioPerformanceService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetPerformance(_FromDate, _ToDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 35.0d)]
        public async void TestPortfolioSummaryServicePerformance()
        {
            var service = new PortfolioSummaryService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetSummary(_AtDate);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 8.0d)]
        public async void TestPortfolioValueServicePerformance()
        {
            var service = new PortfolioValueService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetPortfolioValue(_FromDate, _ToDate, ValueFrequency.Daily);

            _Counter.Increment();
        }

        [PerfBenchmark(Description = "Test to ensure that a minimal throughput test can be rapidly executed.",
        NumberOfIterations = 3, RunMode = RunMode.Throughput,
        RunTimeMilliseconds = 1000, TestMode = TestMode.Test)]
        [CounterThroughputAssertion("TestCounter", MustBe.GreaterThan, 10.0d)]
        public async void TestTransactionsServicePerformance()
        {
            var service = new TransactionService(_PortfolioDatabase, _StockDatabase);
            var responce = await service.GetTransactions(_FromDate, _ToDate);

            _Counter.Increment();
        }


    }
}
