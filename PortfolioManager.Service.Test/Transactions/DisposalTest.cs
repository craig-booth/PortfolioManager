using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Test.Transactions
{

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, sell all")]
    public class DisposalOrdinaryShareSingleParcelSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedCGTEvents.Add(new CGTEvent( _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 1690.00m));
        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, sell part")]
    public class DisposalOrdinaryShareSingleParcelSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Units = 500,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 500, 1.50m, 750.00m, 750.00m, Guid.Empty, ParcelEvent.Disposal));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 500, 750.00m, 840.00m));

        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, multiple dated records, sell part")]
    public class DisposalOrdinaryShareSingleParcelMultipleEffectiveDatesSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = ""
                },
                new ReturnOfCapital()
                {
                    TransactionDate = aquisitionDate,
                    RecordDate = aquisitionDate,
                    ASXCode = "AAA",
                    Amount = 0.20m,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Units = 500,
                    AveragePrice = 1.20m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // costbase prior to sale = (1500 - (1000 * 0.20)) = (1500 - 200) = 1300
            // costbase of sold shares = (1300 / 2) = 650
            // costbase of remainging shares = 1300 - 650 = 650
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 500, 1.50m, 750.00m, 650.00m, Guid.Empty, ParcelEvent.Disposal));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 500, 650.00m, 590.00m));

        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - multiple parcels, sell all")]
    public class DisposalOrdinaryShareMultipleParcelsSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate1 = new DateTime(2000, 01, 01);
            var aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    Comment = ""
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Units = 1500,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 1693.33m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 500, 1200.00m, 846.67m));
        }
    }

    [TestFixture, Description("Disposal of Ordinary Share - multiple parcels, sell part")]
    public class DisposalOrdinaryShareMultipleParcelsSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate1 = new DateTime(2000, 01, 01);
            var aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    Comment = ""
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Units = 1200,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate2, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, 300, 2.40m, 720.00m, 720.00m, Guid.Empty, ParcelEvent.Disposal));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 1691.67m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 200, 480.00m, 338.33m));

        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, sell all")]
    public class DisposalStapledSecuritySingleParcelSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = "Test Opening Balance"
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    AveragePrice = 17.00m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // Relative purchase NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            // Relative sale NTA... s1 = 20% ,s2 = 40%, s3 = 40%
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 3398.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, _TransactionDate, 1000, 4500.00m, 6796.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, _TransactionDate, 1000, 9000.00m, 6796.00m));
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, sell part")]
    public class DisposalStapledSecuritySingleParcelSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = "Test Opening Balance"
                },
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "SSS",
                    Units = 500,
                    AveragePrice = 17.00m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, 500, 1.50m, 750.00m, 750.00m, purchaseId, ParcelEvent.Disposal));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, 500, 4.50m, 2250.00m, 2250.00m, purchaseId, ParcelEvent.Disposal));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, 500, 9.00m, 4500.00m, 4500.00m, purchaseId, ParcelEvent.Disposal));

            // Relative purchase NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            // Relative sale NTA... s1 = 20% ,s2 = 40%, s3 = 40%
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, _TransactionDate, 500, 750.00m, 1698.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, _TransactionDate, 500, 2250.00m, 3396.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, _TransactionDate, 500, 4500.00m, 3396.00m));
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, multiple dated records, sell part")]
    public class DisposalStapledSecuritySingleParcelMultipleEffectiveDatesSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);

            var taxDeferredIncome = new IncomeReceived()
                {
                    TransactionDate = new DateTime(2001, 01, 01),
                    ASXCode = "SSS3",
                    RecordDate = new DateTime(2001, 01, 01),
                    FrankedAmount = 0.00m,
                    UnfrankedAmount = 100.00m,
                    FrankingCredits = 0.00m,
                    Interest = 10.00m,
                    TaxDeferred = 100.00m,
                    Comment = "Income test"
                };

            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = "Test Opening Balance"
                },
                taxDeferredIncome,
                new Disposal()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "SSS",
                    Units = 500,
                    AveragePrice = 17.00m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, 500, 1.50m, 750.00m, 750.00m, purchaseId, ParcelEvent.Disposal));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, 500, 4.50m, 2250.00m, 2250.00m, purchaseId, ParcelEvent.Disposal));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateTimeConstants.NoEndDate, aquisitionDate, _StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, 500, 9.00m, 4500.00m, 4450.00m, purchaseId, ParcelEvent.Disposal));

            // Relative purchase NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            // Relative sale NTA... s1 = 20% ,s2 = 40%, s3 = 40%
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, _TransactionDate, 500, 750.00m, 1698.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, _TransactionDate, 500, 2250.00m, 3396.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, _TransactionDate, 500, 4450.00m, 3396.00m));

            _ExpectedIncome.Add(new Income(taxDeferredIncome));
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - multiple parcels, sell all")]
    [Ignore]
    public class DisposalStapledSecurityMultipleParcelsSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {

        }
    }

    [TestFixture, Description("Disposal of Stapled Security - multiple parcels, sell part")]
    [Ignore]
    public class DisposalStapledSecurityMultipleParcelsSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
        
        }
    }

    [TestFixture, Description("Disposal validation tests")]
    public class DisposalValidationTests : PortfolioTest
    {
        [Test, Description("Disposal of Ordinary Share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var  transactionDate = new DateTime(2002, 01, 01);

            var transactions = new Transaction[]
            {
                new Disposal()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }

        [Test, Description("Disposal of child security")]
        [ExpectedException(typeof(TransctionNotSupportedForChildSecurity))]
        public void NotSupportedForChildSecurity()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var transactions = new Transaction[]
            {
                new Disposal()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "SSS1",
                    Units = 1000,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }


        [Test, Description("Disposal of Ordinary Share - single parcel, not enough shares")]
        [ExpectedException(typeof(NotEnoughSharesForDisposal))]
        public void SingleParcelNotEnoughShares()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Units = 1500,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }

        [Test, Description("Disposal of Ordinary Share - multiple parcels, not enough shares")]
        [ExpectedException(typeof(NotEnoughSharesForDisposal))]
        public void MultipleParcelsNotEnoughShares()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate1 = new DateTime(2000, 01, 01);
            var aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    Comment = ""
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    Comment = ""
                },
                new Disposal()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Units = 2500,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    CGTMethod = CGTCalculationMethod.MinimizeGain,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }
    }
}
