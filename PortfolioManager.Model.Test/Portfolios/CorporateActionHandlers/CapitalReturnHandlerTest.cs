using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnitExtension;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;


namespace PortfolioManager.Model.Test.Portfolios.CorporateActionHandlers
{
    [TestFixture, Description("Capital Return Handler Tests")]
    public class CapitalReturnHandlerTest : PortfolioTest
    {
        [Test, Description("Single Parcel")]
        public void SingleParcel()
        {
            var purchaseDate = new DateTime(2000, 01, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var paymentDate = new DateTime(2010, 02, 01);

            var transactions = new ITransaction[]
            {
                new Aquisition()
                {
                    TransactionDate = purchaseDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 10.00m,
                    TransactionCosts = 19.95m,
                    Comment = "Test aquisition"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            var capitalReturn = new CapitalReturn(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(capitalReturn);

            var expectedTransactions = new ITransaction[]
            {
                new ReturnOfCapital()
                {
                    TransactionDate = paymentDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = 0.50m,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("Multiple Parcels")]
        public void MultipleParcels()
        {
            var purchaseDate1 = new DateTime(2000, 01, 01);
            var purchaseDate2 = new DateTime(2005, 06, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var paymentDate = new DateTime(2010, 02, 01);

            var transactions = new ITransaction[]
            {
                new Aquisition()
                {
                    TransactionDate = purchaseDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 10.00m,
                    TransactionCosts = 19.95m,
                    Comment = "Test aquisition"
                },
                new Aquisition()
                {
                    TransactionDate = purchaseDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    AveragePrice = 15.00m,
                    TransactionCosts = 19.95m,
                    Comment = "Test aquisition"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            var capitalReturn = new CapitalReturn(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(capitalReturn);

            var expectedTransactions = new ITransaction[]
            {
                new ReturnOfCapital()
                {
                    TransactionDate = paymentDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = 0.50m,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("No Parcels")]
        public void NoParcels()
        {
            var recordDate = new DateTime(2010, 01, 01);
            var paymentDate = new DateTime(2010, 02, 01);


            var capitalReturn = new CapitalReturn(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(capitalReturn);

            var expectedTransactions = new ITransaction[]
            {
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

    }

}
