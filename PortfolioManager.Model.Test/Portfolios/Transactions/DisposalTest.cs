﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios.Transactions
{

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, sell all")]
    public class DisposalOrdinaryShareSingleParcelSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
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
            _Portfolio.Transactions.Add(transactions);

            _ExpectedCGTEvents.Add(new CGTEvent( _StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 1690.00m));
        }

    }

    [TestFixture, Description("Disposal of Ordinary Share - single parcel, sell part")]
    public class DisposalOrdinaryShareSingleParcelSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
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
            _Portfolio.Transactions.Add(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 500, 1.50m, 750.00m, 750.00m, ParcelEvent.Disposal)
                {
                    FromDate = _TransactionDate
                });
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 500, 750.00m, 840.00m));

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
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = ""
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
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
            _Portfolio.Transactions.Add(transactions);

            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 1693.33m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 500, 1200.00m, 846.67m));
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
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = ""
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
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
            _Portfolio.Transactions.Add(transactions);

            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockManager.GetStock("AAA", _TransactionDate).Id, 300, 2.40m, 720.00m, 720.00m, ParcelEvent.Disposal)
            {
                FromDate = _TransactionDate
            });

            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 1691.67m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("AAA", _TransactionDate).Id, _TransactionDate, 200, 480.00m, 338.33m));

        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, sell all")]
    public class DisposalStapledSecuritySingleParcelSellAll : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
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
            _Portfolio.Transactions.Add(transactions);

            // Relative purchase NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            // Relative sale NTA... s1 = 20% ,s2 = 40%, s3 = 40%
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("SSS1", _TransactionDate).Id, _TransactionDate, 1000, 1500.00m, 3398.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("SSS2", _TransactionDate).Id, _TransactionDate, 1000, 4500.00m, 6796.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("SSS3", _TransactionDate).Id, _TransactionDate, 1000, 9000.00m, 6796.00m));
        }
    }

    [TestFixture, Description("Disposal of Stapled Security - single parcel, sell part")]
    public class DisposalStapledSecuritySingleParcelSellPart : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
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
            _Portfolio.Transactions.Add(transactions);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var mainParcel = new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS", _TransactionDate).Id, 500, 15.00m, 7500.00m, 7500.00m, ParcelEvent.Disposal);
            _ExpectedParcels.Add(mainParcel);
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS1", _TransactionDate).Id, 500, 1.50m, 750.00m, 750.00m, mainParcel.Id, ParcelEvent.Disposal));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS2", _TransactionDate).Id, 500, 4.50m, 2250.00m, 2250.00m, mainParcel.Id, ParcelEvent.Disposal));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS3", _TransactionDate).Id, 500, 9.00m, 4500.00m, 4500.00m, mainParcel.Id, ParcelEvent.Disposal));

            // Relative purchase NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            // Relative sale NTA... s1 = 20% ,s2 = 40%, s3 = 40%
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("SSS1", _TransactionDate).Id, _TransactionDate, 500, 750.00m, 1699.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("SSS2", _TransactionDate).Id, _TransactionDate, 500, 2250.00m, 3398.00m));
            _ExpectedCGTEvents.Add(new CGTEvent(_StockManager.GetStock("SSS3", _TransactionDate).Id, _TransactionDate, 500, 4500.00m, 3398.00m));
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
    public class DisposalValidationTests : TransactionTest
    {
        [Test, Description("Disposal of Ordinary Share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var  transactionDate = new DateTime(2002, 01, 01);

            var transactions = new ITransaction[]
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
            _Portfolio.Transactions.Add(transactions);
        }

        [Test, Description("Disposal of Ordinary Share - single parcel, not enough shares")]
        [ExpectedException(typeof(NotEnoughSharesForDisposal))]
        public void SingleParcelNotEnoughShares()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
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
            _Portfolio.Transactions.Add(transactions);
        }

        [Test, Description("Disposal of Ordinary Share - multiple parcels, not enough shares")]
        [ExpectedException(typeof(NotEnoughSharesForDisposal))]
        public void MultipleParcelsNotEnoughShares()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate1 = new DateTime(2000, 01, 01);
            var aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = ""
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
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
            _Portfolio.Transactions.Add(transactions);
        }
    }
}
