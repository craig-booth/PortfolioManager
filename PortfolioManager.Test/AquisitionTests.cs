using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Common;
using PortfolioManager.Data;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Local;

namespace PortfolioManager.Test
{
    [TestFixture]
    public class AquisitionTests
    {

        [Test]
        public void Test1()
        {
            var portfolioDatabase =  new SQLitePortfolioDatabase(":memory:");
            var stockDatabase = new SQLiteStockDatabase(":memory:");

            var transactionService = new TransactionService(portfolioDatabase, stockDatabase);

            var transaction = new AquisitionTransactionItem()
            {
                Stock = new StockItem(Guid.Empty, "ABC", "ABC Company"),
                Comment = "Test",
                Units = 100,
                AveragePrice = 12.00m,
                TransactionCosts = 19.95m,
                CreateCashTransaction = true
            };
            transactionService.AddTransaction(transaction);
        }
    }
}
