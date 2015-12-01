using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios.Transactions
{
    [TestFixture, Description("Unit count adjustment of Ordinary share - single parcel")]
    public class UnitCountAdjustmentOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
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
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 2000, 1.50m, 1500.00m, 450.00m, ParcelEvent.UnitCountChange)
            {
                FromDate = _TransactionDate
            }); 
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
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Costbase Adjustment test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 500,
                    CostBase = 1200.00m,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockManager.GetStock("AAA", _TransactionDate).Id, 2000, 1.50m, 1500.00m, 450.00m, ParcelEvent.UnitCountChange)
            {
                FromDate = _TransactionDate
            });
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 2.40m, 1200.00m, 360.00m, ParcelEvent.UnitCountChange)
            {
                FromDate = _TransactionDate
            });
                   
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
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate1,
                    ASXCode = "AAA",
                    Units = 100,
                    CostBase = 1500.00m,
                    Comment = "Costbase Adjustment test"
                },
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate2,
                    ASXCode = "AAA",
                    Units = 200,
                    CostBase = 1200.00m,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    NewUnits = 3,
                    OriginalUnits = 1,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockManager.GetStock("AAA", _TransactionDate).Id, 67, 1.50m, 1500.00m, 450.00m, ParcelEvent.UnitCountChange)
            {
                FromDate = _TransactionDate
            });
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockManager.GetStock("AAA", _TransactionDate).Id, 33, 2.40m, 1200.00m, 360.00m, ParcelEvent.UnitCountChange)
            {
                FromDate = _TransactionDate
            });
        }
    }


    [TestFixture, Description("Unit Count adjustment validation tests")]
    public class UnitCountAdjustmentValidationTests : TransactionTest
    {
        [Test, Description("Cost base adjustment of Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var transactions = new ITransaction[]
            {
                new UnitCountAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "AAA",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions); 
        }

        [Test]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForStapledSecurity()
        {
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions); 
        }

        [Test]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForChildSecurity()
        {
            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Costbase Adjustment test"
                },
                new UnitCountAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS1",
                    NewUnits = 1,
                    OriginalUnits = 2,
                    Comment = "Unit Count Adjustment test"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }
    }
}
