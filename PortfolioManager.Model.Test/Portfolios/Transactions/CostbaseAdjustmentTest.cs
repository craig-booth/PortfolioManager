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

    [TestFixture, Description("Cost base adjustment of Ordinary share - single parcel")]
    public class CostBaseAdjustmentOrdinaryShareSingleParcel : TransactionTest
    {
        public override void Setup()
        {
            SetupTestPortfolio();

            _TransactionDate = new DateTime(2002, 01, 01);

            var costBaseAdjustment = new CostBaseAdjustment()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                Percentage = 0.30m,
                Comment = "Costbase Adjustment test"
            };
            _Portfolio.Transactions.Add(costBaseAdjustment);

            var expectedParcels = new ShareParcel[]
            {
                
            };

            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockManager.GetStock("AAA", _TransactionDate).Id, 1000, 1.50m, 1500.00m, 450.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = _TransactionDate
                });
        }
    }

    [TestFixture]
    public class CostBaseAdjustmentTest : PortfolioTestBase
    {
        [Test, Description("Cost base adjustment of Ordinary share - single parcel")]
        public void OrdinaryShareSingleParcel()
        {
            // Setup
            var testPortfolio = CreateTestPortfolio();

            DateTime aquisitionDate = new DateTime(2000, 01, 01);
            DateTime transactionDate = new DateTime(2002, 01, 01);

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
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            testPortfolio.Transactions.Add(transactions);

            var actualParcels = testPortfolio.GetParcels(transactionDate);

            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(aquisitionDate, _AAAId, 1000, 1.50m, 1500.00m, 450.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = transactionDate
                }
            };

            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }


        [Test, Description("Cost base adjustment of Ordinary share - multiple parcels")]
        public void OrdinaryShareMultipleParcels()
        {
            var testPortfolio = CreateTestPortfolio();

            DateTime aquisitionDate1 = new DateTime(2000, 01, 01);
            DateTime aquisitionDate2 = new DateTime(2001, 01, 01);
            DateTime transactionDate = new DateTime(2002, 01, 01);

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
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            testPortfolio.Transactions.Add(transactions);

            var actualParcels = testPortfolio.GetParcels(transactionDate);

            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(aquisitionDate1, _AAAId, 1000, 1.50m, 1500.00m, 450.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = transactionDate
                },
                new ShareParcel(aquisitionDate2, _AAAId, 500, 2.40m, 1200.00m, 360.00m, ParcelEvent.CostBaseReduction)
                {
                    FromDate = transactionDate
                }
            };

            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Cost base adjustment of Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void OrdinaryNoParcels()
        {
            var testPortfolio = CreateTestPortfolio();

            DateTime transactionDate = new DateTime(2002, 01, 01);

            var transactions = new ITransaction[]
            {              
                new CostBaseAdjustment()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "AAA",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            testPortfolio.Transactions.Add(transactions);
        }

        [Test, Description("Cost base adjustment of Stapled Security")]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void StapledSecurtiy()
        {
            var testPortfolio = CreateTestPortfolio();

            DateTime aquisitionDate = new DateTime(2000, 01, 01);
            DateTime transactionDate = new DateTime(2002, 01, 01);

            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 1500.00m,
                    Comment = "Costbase Adjustment test"
                },
                new CostBaseAdjustment()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "SSS",
                    Percentage = 0.30m,
                    Comment = "Costbase Adjustment test"
                }
            };
            testPortfolio.Transactions.Add(transactions);
        }
    }
}
