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
    [TestFixture, Description("Unit count adjustment of Ordinary share - single parcel")]
    public class UnitCountAdjustmentOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
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
                    RecordDate = aquisitionDate,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    NewUnits = 2,
                    OriginalUnits = 1,
                    RecordDate = _TransactionDate,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate, GetStockId("AAA"), 2000, 1.50m, 1500.00m, 1500.00m, Guid.Empty)); 
        }
    }

    [TestFixture, Description("Unit count adjustment of Ordinary share - multiple parcels")]
    public class UnitCountAdjustmentOrdinaryShareMultipleParcels : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            DateTime aquisitionDate1 = new DateTime(2000, 01, 01);
            DateTime aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    RecordDate = aquisitionDate1,
                    Comment = "Costbase Adjustment test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    RecordDate = aquisitionDate2,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    NewUnits = 2,
                    OriginalUnits = 1,
                    RecordDate = _TransactionDate,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate1, GetStockId("AAA"), 2000, 1.50m, 1500.00m, 1500.00m, Guid.Empty));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate2, GetStockId("AAA"), 1000, 2.40m, 1200.00m, 1200.00m, Guid.Empty));
                   
        }
    }


    [TestFixture, Description("Unit count adjustment of Ordinary share - multiple parcels, uneven amount")]
    public class UnitCountAdjustmentOrdinaryShareMultipleParcelsUnevenAmount : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            DateTime aquisitionDate1 = new DateTime(2000, 01, 01);
            DateTime aquisitionDate2 = new DateTime(2001, 01, 01);
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 100,
                    CostBase = 1500.00m,
                    AquisitionDate = aquisitionDate1,
                    RecordDate = aquisitionDate1,
                    Comment = "Costbase Adjustment test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 200,
                    CostBase = 1200.00m,
                    AquisitionDate = aquisitionDate2,
                    RecordDate = aquisitionDate2,
                    Comment = "Costbase Adjustment test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 110,
                    CostBase = 1100.00m,
                    AquisitionDate = aquisitionDate2,
                    RecordDate = aquisitionDate2,
                    Comment = "Costbase Adjustment test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 131,
                    CostBase = 1310.00m,
                    AquisitionDate = aquisitionDate2,
                    RecordDate = aquisitionDate2,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    NewUnits = 1,
                    OriginalUnits = 3,
                    RecordDate = _TransactionDate,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate1, GetStockId("AAA"), 33, 15.00m, 1500.00m, 1500.00m, Guid.Empty));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate2, GetStockId("AAA"), 67, 6.00m, 1200.00m, 1200.00m, Guid.Empty));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate2, GetStockId("AAA"), 37, 10.00m, 1100.00m, 1100.00m, Guid.Empty));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate2, GetStockId("AAA"), 43, 10.00m, 1310.00m, 1310.00m, Guid.Empty));
        }
    }

    [TestFixture, Description("Unit Count adjustment of Child security - single parcels")]
    public class UnitCountAdjustmentChildSecuritySingleParcel : TransactionTestWithExpectedTests
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
                    RecordDate = aquisitionDate,
                    Comment = ""
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "SSS1",
                    NewUnits = 2,
                    OriginalUnits = 1,
                    RecordDate = _TransactionDate,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate, GetStockId("SSS1"), 2000, 1.50m, 1500.00m, 1500.00m, purchaseId));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, GetStockId("SSS2"), 1000, 4.50m, 4500.00m, 4500.00m, purchaseId));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, GetStockId("SSS3"), 1000, 9.00m, 9000.00m, 9000.00m, purchaseId));

        }
    }


    [TestFixture, Description("Unit Count adjustment validation tests")]
    public class UnitCountAdjustmentValidationTests : PortfolioTest
    {
        [Test, Description("Cost base adjustment of Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var transactions = new Transaction[]
            {
                new UnitCountAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "AAA",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    RecordDate = new DateTime(2000, 01, 01),
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions); 
        }

        [Test]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForStapledSecurity()
        {
            var transactions = new Transaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 1500.00m,
                    AquisitionDate = new DateTime(2000, 01, 01),
                    RecordDate = new DateTime(2000, 01, 01),
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    RecordDate = new DateTime(2000, 01, 01),
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions); 
        }

    }
}
