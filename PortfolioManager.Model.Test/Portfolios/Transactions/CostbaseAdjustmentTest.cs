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

    [TestFixture, Description("Cost base adjustment of Ordinary share - single parcel")]
    public class CostBaseAdjustmentOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
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
                new CostBaseAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            _Portfolio.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 450.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = _TransactionDate
                });
        }
    }

    [TestFixture, Description("Cost base adjustment of Ordinary share - multiple parcels")]
    public class CostBaseAdjustmentOrdinaryShareMultipleParcels : TransactionTestWithExpectedTests
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
                new CostBaseAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "AAA",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            _Portfolio.ProcessTransactions(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate1, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 450.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = _TransactionDate
                });
             _ExpectedParcels.Add(new ShareParcel(aquisitionDate2, _StockManager.GetStock("AAA", _TransactionDate).Id, 500, 2.40m, 1200.00m, 360.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = _TransactionDate
                });                           

        }
    }

    [TestFixture, Description("Cost base adjustment of Child security - single parcels")]
    public class CostBaseAdjustmentChildSecuritySingleParcel : TransactionTestWithExpectedTests
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
                    Comment = ""
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = _TransactionDate,
                    ASXCode = "SSS3",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                } 
            };
            _Portfolio.ProcessTransactions(transactions);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var mainParcel = new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS", _TransactionDate).Id, 1000, 15.00m, 15000.00m, 8700.00m, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            };
            _ExpectedParcels.Add(mainParcel);
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS1", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 1500.00m, mainParcel.Id, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS2", _TransactionDate).Id, 1000, 4.50m, 4500.00m, 4500.00m, mainParcel.Id, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("SSS3", _TransactionDate).Id, 1000, 9.00m, 9000.00m, 2700.00m, mainParcel.Id, ParcelEvent.CostBaseReduction)
            {
                FromDate = _TransactionDate
            });

        }
    }

    [TestFixture, Description("Cost base adjustment validation tests")]
    public class CostBaseAdjustmentValidationTests : TransactionTest
    {
        [Test, Description("Cost base adjustment of Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var transactions = new ITransaction[]
            {           
                new CostBaseAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "AAA",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            _Portfolio.ProcessTransactions(transactions);
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
                new CostBaseAdjustment()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "SSS",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            _Portfolio.ProcessTransactions(transactions);
        }
    }

}
