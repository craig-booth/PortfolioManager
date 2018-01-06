using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using NUnit.Framework;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Local;

namespace PortfolioManager.Test.SystemTests
{
    [TestFixture]
    public class CompareToLive
    {
        private IPortfolioDatabase _PortfolioDatabase;
        private IStockDatabase _StockDatabase;

        [OneTimeSetUp]
        public void Init()
        {
            var path = @"C:\PortfolioManager\UnitTesting";
            _PortfolioDatabase = new SQLitePortfolioDatabase(Path.Combine(path, "Natalies Portfolio.db"));
            _StockDatabase = new SQLiteStockDatabase(Path.Combine(path, "Stocks.db"));

            /*      var transactionService = new TransactionService(_PortfolioDatabase, _StockDatabase);

                  var transaction = new AquisitionTransactionItem()
                  {
                      Stock = new StockItem(Guid.Empty, "ABC", "ABC Company"),
                      Comment = "Test",
                      Units = 100,
                      AveragePrice = 12.00m,
                      TransactionCosts = 19.95m,
                      CreateCashTransaction = true
                  };
                  transactionService.AddTransaction(transaction); */
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task ComparePortfolioSummary(DateTime date)
        {
            var service = new PortfolioSummaryService(_PortfolioDatabase, _StockDatabase);

            var responce = await service.GetSummary(date);

            var fileName = String.Format("PortfolioSummary {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "ExpectedResults", fileName);

            Assert.That(responce, Is.EquivalentTo(typeof(PortfolioSummaryResponce), expectedFile));
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
                    var testData = new TestCaseData(date);       
                    date = date.AddMonths(1);

                    yield return testData;
                }
            }
        }
    }
}
