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

namespace PortfolioManager.Service.Test.CorporateActionHandlers
{
    [TestFixture, Description("Transformation Handler Tests")]
    public class TransformationHandlerTest : PortfolioTest
    {
        [Test, Description("Single Parcel")]
        public void SingleParcel()
        {
            var purchaseDate = new DateTime(2000, 01, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var implemenationDate = new DateTime(2010, 02, 05);

            var transactions = new Transaction[]
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

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implemenationDate, 0.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(transformation);

            var expectedTransactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate,
                    Units = 1500,
                    CostBase = 0.20m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Percentage = 0.80m,
                    Comment = "Test"
                }

            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("Multiple Parcels, Rollover, multiple result stocks")]
        public void MultipleParcelsRolloverMultipleResultStocks()
        {
            var purchaseDate1 = new DateTime(2000, 01, 01);
            var purchaseDate2 = new DateTime(2005, 06, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var implemenationDate = new DateTime(2010, 02, 05);

            var transactions = new Transaction[]
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

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implemenationDate, 0.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("CCC", recordDate).Id, 1, 2, 0.30m);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(transformation);

            var expectedTransactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate1,
                    Units = 1500,
                    CostBase = 0.20m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "CCC",
                    AquisitionDate = purchaseDate1,
                    Units = 2000,
                    CostBase = 0.30m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate2,
                    Units = 750,
                    CostBase = 0.20m * (500 * 15.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "CCC",
                    AquisitionDate = purchaseDate2,
                    Units = 1000,
                    CostBase = 0.30m * (500 * 15.00m + 19.95m),
                    Comment = "Test"
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Percentage = 0.50m,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("Multiple Parcels, Rollover, Disposal of original stock")]
        public void MultipleParcelsRolloverDisposalOfOriginalStock()
        {
            var purchaseDate1 = new DateTime(2000, 01, 01);
            var purchaseDate2 = new DateTime(2005, 06, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var implemenationDate = new DateTime(2010, 02, 05);

            var transactions = new Transaction[]
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

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implemenationDate, 12.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("CCC", recordDate).Id, 1, 2, 0.30m);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(transformation);

            var expectedTransactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate1,
                    Units = 1500,
                    CostBase = 0.20m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "CCC",
                    AquisitionDate = purchaseDate1,
                    Units = 2000,
                    CostBase = 0.30m * (1000 * 10.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "BBB",
                    AquisitionDate = purchaseDate2,
                    Units = 750,
                    CostBase = 0.20m * (500 * 15.00m + 19.95m),
                    Comment = "Test"
                },
                new OpeningBalance()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "CCC",
                    AquisitionDate = purchaseDate2,
                    Units = 1000,
                    CostBase = 0.30m * (500 * 15.00m + 19.95m),
                    Comment = "Test"
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Percentage = 0.50m,
                    Comment = "Test"
                },
                new Disposal()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    Units = 1500,
                    AveragePrice = 12.00m,
                    TransactionCosts = 0.00m,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));

        }

        [Test, Description("Multiple Parcels, No Rollover, Multiple result stocks")]
        public void MultipleParcelsNoRolloverMultipleResultStocks()
        {
            var purchaseDate1 = new DateTime(2000, 01, 01);
            var purchaseDate2 = new DateTime(2005, 06, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var implemenationDate = new DateTime(2010, 02, 05);
            var bbbAquistionDate = new DateTime(2010, 02, 01);
            var cccAquistionDate = new DateTime(2010, 02, 07);

            var transactions = new Transaction[]
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

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implemenationDate, 0.00m, false, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 1.00m, bbbAquistionDate);
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("CCC", recordDate).Id, 1, 2, 2.30m, cccAquistionDate);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(transformation);

            var expectedTransactions = new Transaction[]
            {
                new Aquisition()
                {
                    TransactionDate = bbbAquistionDate,
                    ASXCode = "BBB",
                    Units = 2250,
                    AveragePrice = 1.00m,
                    TransactionCosts = 0.00m,
                    Comment = "Test"
                },
                new Aquisition()
                {
                    TransactionDate = cccAquistionDate,
                    ASXCode = "CCC",
                    Units = 3000,
                    AveragePrice = 2.30m,
                    TransactionCosts = 0.00m,
                    Comment = "Test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = (2250 * 1.00m) + (3000 * 2.30m),
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));

        }

        [Test, Description("Multiple Parcels, Rollover, Disposal of original stock")]
        public void MultipleParcelsNoRolloverDisposalOfOriginalStock()
        {
            var purchaseDate1 = new DateTime(2000, 01, 01);
            var purchaseDate2 = new DateTime(2005, 06, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var implemenationDate = new DateTime(2010, 02, 05);
            var bbbAquistionDate = new DateTime(2010, 02, 01);
            var cccAquistionDate = new DateTime(2010, 02, 07);

            var transactions = new Transaction[]
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

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implemenationDate, 4.00m, false, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 1.00m, bbbAquistionDate);
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("CCC", recordDate).Id, 1, 2, 2.30m, cccAquistionDate);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(transformation);

            var expectedTransactions = new Transaction[]
            {
                new Aquisition()
                {
                    TransactionDate = bbbAquistionDate,
                    ASXCode = "BBB",
                    Units = 2250,
                    AveragePrice = 1.00m,
                    TransactionCosts = 0.00m,
                    Comment = "Test"
                },
                new Aquisition()
                {
                    TransactionDate = cccAquistionDate,
                    ASXCode = "CCC",
                    Units = 3000,
                    AveragePrice = 2.30m,
                    TransactionCosts = 0.00m,
                    Comment = "Test"
                },
                new ReturnOfCapital()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = (2250 * 1.00m) + (3000 * 2.30m),
                    Comment = "Test"
                },
                new Disposal()
                {
                    TransactionDate = implemenationDate,
                    ASXCode = "AAA",
                    Units = 1500,
                    AveragePrice = 4.00m,
                    TransactionCosts = 0.00m,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    Comment = "Test"
                }
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

        [Test, Description("No Parcels")]
        public void NoParcels()
        {
            var recordDate = new DateTime(2010, 01, 01);
            var implemenationDate = new DateTime(2010, 02, 05);

            var transformation = new Transformation(_StockServiceRepository.StockService.GetStock("AAA", recordDate).Id, recordDate, implemenationDate, 0.00m, true, "Test");
            transformation.AddResultStock(_StockServiceRepository.StockService.GetStock("BBB", recordDate).Id, 2, 3, 0.20m);

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(transformation);

            var expectedTransactions = new Transaction[]
            {
            };

            Assert.That(actualTransactions, EntityConstraint.CollectionEqual(expectedTransactions));
        }

    }

}
