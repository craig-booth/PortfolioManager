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
        public override void Setup()
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
            _Portfolio.Transactions.Add(transactions);


            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 450.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = _TransactionDate
                });
        }
    }

    [TestFixture, Description("Cost base adjustment of Ordinary share - multiple parcels")]
    public class CostBaseAdjustmentOrdinaryShareMultipleParcels : TransactionTestWithExpectedTests
    {
        public override void Setup()
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
            _Portfolio.Transactions.Add(transactions);


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

    [TestFixture, Description("Cost base adjustment of Ordinary share - no parcels")]
    public class CostBaseAdjustmentOrdinaryShareNoParcels : TransactionTest
    {
        [Test]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcels()
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
            _Portfolio.Transactions.Add(transactions);
        }
    }

    [TestFixture, Description("Cost base adjustment of Stapled Security")]    
    public class CostBaseAdjustmentStapledSecurity : TransactionTest
    {
        [Test]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void StapledSecurity()
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
            _Portfolio.Transactions.Add(transactions);
        }
    }

}
