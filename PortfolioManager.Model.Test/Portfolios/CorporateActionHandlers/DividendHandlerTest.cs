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
    [TestFixture, Description("Dividend Handler Tests")]
    public class DividendHandlerTest : PortfolioTest
    {
        [Test, Description("Single Parcel, No DRP")]
        public void SingleParcelNoDRP()
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


            var dividend = new Dividend(_StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, 0.50m, 0.300m, 0.00m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(dividend);

            var expectedTransactions = new ITransaction[]
            {
                new IncomeReceived()
                {
                    TransactionDate = paymentDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    FrankedAmount = (1000 * 0.50m) * 0.50m,
                    UnfrankedAmount = (1000 * 0.50m) * 0.50m,
                    FrankingCredits = 107.14m,
                    Interest = 0.00m,
                    TaxDeferred = 0.00m,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("Multiple Parcels, No DRP")]
        public void MultipleParcelsNoDRP()
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

            var dividend = new Dividend(_StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, 0.80m, 0.30m, 0.00m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(dividend);

            var expectedTransactions = new ITransaction[]
            {
                new IncomeReceived()
                {
                    TransactionDate = paymentDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    FrankedAmount = (1500 * 0.50m) * 0.80m,
                    UnfrankedAmount = (1500 * 0.50m) * 0.20m,
                    FrankingCredits = 257.14m,
                    Interest = 0.00m,
                    TaxDeferred = 0.00m,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("Multiple Parcels, DRP")]
        public void MultipleParcelsDRP()
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
            _Portfolio.StockSetting.Add("AAA", new StockSetting("AAA") { DRPActive = true });

            var dividend = new Dividend(_StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, 1.00m, 0.30m, 20.00m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(dividend);

            var expectedTransactions = new ITransaction[]
            {
                new IncomeReceived()
                {
                    TransactionDate = paymentDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    FrankedAmount = 1500 * 0.50m,
                    UnfrankedAmount = 0.00m,
                    FrankingCredits =  321.43m,
                    Interest = 0.00m,
                    TaxDeferred = 0.00m,
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = paymentDate,
                    AquisitionDate = paymentDate,
                    ASXCode = "AAA",
                    Units = 38,
                    CostBase = 1500 * 0.50m,
                    Comment = "DRP $20.00"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }
        
        [Test, Description("No Parcels")]
        public void NoParcels()
        {
            var recordDate = new DateTime(2010, 01, 01);
            var paymentDate = new DateTime(2010, 02, 01);

            var dividend = new Dividend(_StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, 0.50m, 0.30m, 0.00m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(dividend);

            var expectedTransactions = new ITransaction[]
            {
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

    }

}
