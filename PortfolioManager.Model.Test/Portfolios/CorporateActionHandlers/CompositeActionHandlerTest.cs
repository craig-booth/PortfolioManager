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
    [TestFixture, Description("Composite Action Handler Tests")]
    public class CompositeActionHandlerTest : PortfolioTest
    {
        [Test, Description("Single Parcel")]
        public void SingleParcel()
        {
            var purchaseDate = new DateTime(2000, 01, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var implementationDate = new DateTime(2010, 02, 01);

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


            var compositeAction = new CompositeAction(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, "Test");

            var capitalReturn = new CapitalReturn(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implementationDate, 0.50m, "Test");
            compositeAction.Children.Add(capitalReturn);

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implementationDate, 0.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);
            compositeAction.Children.Add(transformation);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(compositeAction);

            var expectedTransactions = new ITransaction[]
            {
                new ReturnOfCapital()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = 0.50m,
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate,
                    Units = 1500,
                    CostBase = 0.20m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Percentage = 0.80m,
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
            var implementationDate = new DateTime(2010, 02, 01);

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

            var compositeAction = new CompositeAction(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, "Test");

            var capitalReturn = new CapitalReturn(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implementationDate, 0.50m, "Test");
            compositeAction.Children.Add(capitalReturn);

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implementationDate, 0.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("CCC", recordDate).Id, 1, 2, 0.30m);
            compositeAction.Children.Add(transformation);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(compositeAction);

            var expectedTransactions = new ITransaction[]
            {
                new ReturnOfCapital()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = 0.50m,
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate1,
                    Units = 1500,
                    CostBase = 0.20m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "CCC",
                    AquisitionDate = purchaseDate1,
                    Units = 2000,
                    CostBase = 0.30m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate2,
                    Units = 750,
                    CostBase = 0.20m * (500 * 15.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "CCC",
                    AquisitionDate = purchaseDate2,
                    Units = 1000,
                    CostBase = 0.30m * (500 * 15.00m + 19.95m),
                    Comment = "Test"
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = implementationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Percentage = 0.50m,
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
            var implementationDate = new DateTime(2010, 02, 01);

            var compositeAction = new CompositeAction(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, "Test");

            var capitalReturn = new CapitalReturn(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implementationDate, 0.50m, "Test");
            compositeAction.Children.Add(capitalReturn);

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implementationDate, 0.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);
            compositeAction.Children.Add(transformation);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(compositeAction);

            var expectedTransactions = new ITransaction[]
            {
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

    }

}

