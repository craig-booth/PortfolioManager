using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;


namespace PortfolioManager.Model.Test.Portfolios.CorporateActionHandlers
{
    [TestFixture, Description("Split/Consolidation Handler Tests")]
    public class SplitConsolidationHandlerTest : PortfolioTest
    {
        [Test, Description("Single Parcel")]
        public void SingleParcel()
        {
            var purchaseDate = new DateTime(2000, 01, 01);
            var recordDate = new DateTime(2010, 01, 01);

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


            var splitConsolidation = new SplitConsolidation(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, 2, 3, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(splitConsolidation);

            var expectedTransactions = new ITransaction[]
            {
                new UnitCountAdjustment()
                {
                    TransactionDate = recordDate,
                    ASXCode = "AAA",
                    OriginalUnits = 2,
                    NewUnits = 3,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, PortfolioConstraint.Equals(expectedTransactions));
        }

        [Test, Description("Multiple Parcels")]
        public void MultipleParcels()
        {
            var purchaseDate1 = new DateTime(2000, 01, 01);
            var purchaseDate2 = new DateTime(2005, 06, 01);
            var recordDate = new DateTime(2010, 01, 01);

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

            var splitConsolidation = new SplitConsolidation(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, 2, 3, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(splitConsolidation);

            var expectedTransactions = new ITransaction[]
            {
                new UnitCountAdjustment()
                {
                    TransactionDate = recordDate,
                    ASXCode = "AAA",
                    OriginalUnits = 2,
                    NewUnits = 3,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, PortfolioConstraint.Equals(expectedTransactions));
        }

        [Test, Description("No Parcels")]
        public void NoParcels()
        {
            var recordDate = new DateTime(2010, 01, 01);

            var splitConsolidation = new SplitConsolidation(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, 2, 3, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(splitConsolidation);

            var expectedTransactions = new ITransaction[]
            {
            };

            Assert.That(actualTransactions, PortfolioConstraint.Equals(expectedTransactions));
        }

    }

}

