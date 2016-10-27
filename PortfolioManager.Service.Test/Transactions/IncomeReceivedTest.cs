﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Test.Transactions
{

    [TestFixture, Description("Income Received of Ordinary share - single parcel")]
    public class IncomeReceivedOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2001, 01, 01);
            
            var openingBalanceDate = new DateTime(2000, 01, 01);
            var openingBalance = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate,
                ASXCode = "AAA",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = openingBalanceDate,
                RecordDate = openingBalanceDate,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                RecordDate = _TransactionDate,
                FrankedAmount = 100.00m,
                UnfrankedAmount = 20.00m,
                FrankingCredits = 30.00m,
                Interest = 0.00m,
                TaxDeferred = 0.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);


            _ExpectedParcels.Add(new ShareParcel(openingBalanceDate, GetStockId("AAA"), openingBalance.Units, 1.50m, openingBalance.CostBase, openingBalance.CostBase));
            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
        }
    }
    [TestFixture, Description("Income Received of Trust- single parcel tax deferred")]
    public class IncomeReceivedTrustSingleParcelTaxDeferred : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2001, 01, 01);

            var openingBalanceDate = new DateTime(2000, 01, 01);
            var openingBalance = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate,
                ASXCode = "CCC",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = openingBalanceDate,
                RecordDate = openingBalanceDate,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "CCC",
                RecordDate = _TransactionDate,
                FrankedAmount = 0.00m,
                UnfrankedAmount = 100.00m,
                FrankingCredits = 0.00m,
                Interest = 10.00m,
                TaxDeferred = 30.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);


            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, openingBalanceDate, GetStockId("CCC"), openingBalance.Units, 1.50m, openingBalance.CostBase, openingBalance.CostBase - 30.00m, Guid.Empty));
            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
        }
    }

    [TestFixture, Description("Income Received of Ordinary share - mulitple parcels")]
    public class IncomeReceivedOrdinaryShareMultipleParcels : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2001, 01, 01);

            var openingBalanceDate1 = new DateTime(2000, 01, 01);
            var openingBalance1 = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate1,
                ASXCode = "AAA",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = openingBalanceDate1,
                RecordDate = openingBalanceDate1,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance1);

            var openingBalanceDate2 = new DateTime(2000, 01, 01);
            var openingBalance2 = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate2,
                ASXCode = "AAA",
                Units = 500,
                CostBase = 800.00m,
                AquisitionDate = openingBalanceDate2,
                RecordDate = openingBalanceDate2,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance2);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                RecordDate = _TransactionDate,
                FrankedAmount = 100.00m,
                UnfrankedAmount = 20.00m,
                FrankingCredits = 30.00m,
                Interest = 0.00m,
                TaxDeferred = 0.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);


            _ExpectedParcels.Add(new ShareParcel(openingBalanceDate1, GetStockId("AAA"), openingBalance1.Units, 1.50m, openingBalance1.CostBase, openingBalance1.CostBase));
            _ExpectedParcels.Add(new ShareParcel(openingBalanceDate2, GetStockId("AAA"), openingBalance2.Units, 1.60m, openingBalance2.CostBase, openingBalance2.CostBase));
            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
        }
    }

    [TestFixture, Description("Income Received of Trust - single parcel tax deferred greater than cost base")]
    public class IncomeReceivedTrustSingleParcelTaxDeferredGreaterThanCostbase : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2001, 01, 01);

            var openingBalanceDate = new DateTime(2000, 01, 01);
            var openingBalance = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate,
                ASXCode = "CCC",
                Units = 100,
                CostBase = 100.00m,
                AquisitionDate = openingBalanceDate,
                RecordDate = openingBalanceDate,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "CCC",
                RecordDate = _TransactionDate,
                FrankedAmount = 0.00m,
                UnfrankedAmount = 100.00m,
                FrankingCredits = 0.00m,
                Interest = 10.00m,
                TaxDeferred = 130.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);


            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, openingBalanceDate, GetStockId("CCC"), openingBalance.Units, 1.00m, openingBalance.CostBase, 0.00m, Guid.Empty));
            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
          //  _ExpectedCGTEvents.Add(new CGTEvent(GetStockId("CCC"), _TransactionDate, openingBalance.Units, openingBalance.CostBase, 30.00m));
        }
    }

    [TestFixture, Description("Income Received of Trust - multiple parcels tax deferred")]
    public class IncomeReceivedTrustMultipleParcelsTaxDeferred : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var openingBalanceDate1 = new DateTime(2000, 01, 01);
            var openingBalance1 = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate1,
                ASXCode = "CCC",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = openingBalanceDate1,
                RecordDate = openingBalanceDate1,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance1);

            var openingBalanceDate2 = new DateTime(2001, 06, 01);
            var openingBalance2 = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate2,
                ASXCode = "CCC",
                Units = 100,
                CostBase = 100.00m,
                AquisitionDate = openingBalanceDate2,
                RecordDate = openingBalanceDate2,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance2);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "CCC",
                RecordDate = _TransactionDate,
                FrankedAmount = 0.00m,
                UnfrankedAmount = 100.00m,
                FrankingCredits = 0.00m,
                Interest = 10.00m,
                TaxDeferred = 300.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);

            /* new cost base 
             * 
             * parcel1 = 1500 - (300 * (1000 / 1100)) = 1500 - 272.73 = 1227.27
             * parcel1 = 100 - (300 * (100 / 1100)) = 100 - 27.27 = 72.73
             * 
            */
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, openingBalanceDate1, GetStockId("CCC"), openingBalance1.Units, 1.50m, openingBalance1.CostBase, 1227.27m, Guid.Empty));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, openingBalanceDate2, GetStockId("CCC"), openingBalance2.Units, 1.00m, openingBalance2.CostBase, 72.73m, Guid.Empty));
            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
        }
    }

    [TestFixture, Description("Income Received of Trust - multiple parcels tax deferred greater than cost base for one parcel")]
    public class IncomeReceivedTrustMultipleParcelsTaxDeferredGreaterThanCostbase : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var openingBalanceDate1 = new DateTime(2000, 01, 01);
            var openingBalance1 = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate1,
                ASXCode = "CCC",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = openingBalanceDate1,
                RecordDate = openingBalanceDate1,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance1);

            var openingBalanceDate2 = new DateTime(2001, 06, 01);
            var openingBalance2 = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate2,
                ASXCode = "CCC",
                Units = 100,
                CostBase = 100.00m,
                AquisitionDate = openingBalanceDate2,
                RecordDate = openingBalanceDate2,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance2);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "CCC",
                RecordDate = _TransactionDate,
                FrankedAmount = 0.00m,
                UnfrankedAmount = 100.00m,
                FrankingCredits = 0.00m,
                Interest = 10.00m,
                TaxDeferred = 1300.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);

            /* new cost base 
             * 
             * parcel1 = 1500 - (1300 * (1000 / 1100)) = 1500 - 1181.82 = 318.18
             * parcel1 = 100 - (1300 * (100 / 1100)) = 100 - 118.18 = -18.18
             * 
            */
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, openingBalanceDate1, GetStockId("CCC"), openingBalance1.Units, 1.50m, openingBalance1.CostBase, 318.18m, Guid.Empty));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, openingBalanceDate2, GetStockId("CCC"), openingBalance2.Units, 1.00m, openingBalance2.CostBase, 0.00m, Guid.Empty));
            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
           // _ExpectedCGTEvents.Add(new CGTEvent(GetStockId("CCC"), _TransactionDate, openingBalance2.Units, openingBalance2.CostBase, 18.18m));
        }
    }

    [TestFixture, Description("Income Received of Child trust - single parcel tax deferred")]
    public class IncomeReceviedChildTrustSingleParcelTaxDeferred : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2002, 01, 01);

            var aquisitionDate = new DateTime(2000, 01, 01);
            var openingBalance = new OpeningBalance()
                {
                    TransactionDate = aquisitionDate,
                    ASXCode = "SSS",
                    Units = 1000,
                    CostBase = 15000.00m,
                    AquisitionDate = aquisitionDate,
                    RecordDate = aquisitionDate,
                    Comment = ""
                };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "SSS3",
                RecordDate = _TransactionDate,
                FrankedAmount = 0.00m,
                UnfrankedAmount = 100.00m,
                FrankingCredits = 0.00m,
                Interest = 10.00m,
                TaxDeferred = 300.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, GetStockId("SSS1"), 1000, 1.50m, 1500.00m, 1500.00m, purchaseId));
            _ExpectedParcels.Add(new ShareParcel(aquisitionDate, GetStockId("SSS2"), 1000, 4.50m, 4500.00m, 4500.00m, purchaseId));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, DateUtils.NoEndDate, aquisitionDate, GetStockId("SSS3"), 1000, 9.00m, 9000.00m, 8700.00m, purchaseId));

            _ExpectedIncome.Add(IncomeFromTransacation(incomeReceived));
        }
    }

    [TestFixture, Description("Income Received validation tests")]
    public class IncomeReceivedValidationTests : PortfolioTest
    {
        [Test, Description("Income Received Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2002, 01, 01),
                ASXCode = "AAA",
                RecordDate = new DateTime(2002, 01, 01),
                FrankedAmount = 100.00m,
                UnfrankedAmount = 20.00m,
                FrankingCredits = 30.00m,
                Interest = 0.00m,
                TaxDeferred = 0.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);
        }

        [Test, Description("Income Received of Stapled Security")]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForStapledSecurity()
        {
            var openingBalanceDate = new DateTime(2000, 01, 01);
            var openingBalance = new OpeningBalance()
            {
                TransactionDate = openingBalanceDate,
                ASXCode = "SSS",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = openingBalanceDate,
                RecordDate = openingBalanceDate,
                Comment = ""
            };
            _Portfolio.TransactionService.ProcessTransaction(openingBalance);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2002, 01, 01),
                ASXCode = "SSS",
                RecordDate = new DateTime(2002, 01, 01),
                FrankedAmount = 100.00m,
                UnfrankedAmount = 20.00m,
                FrankingCredits = 30.00m,
                Interest = 0.00m,
                TaxDeferred = 0.00m,
                Comment = "Income test"
            };
            _Portfolio.TransactionService.ProcessTransaction(incomeReceived);
        }

    }

}
