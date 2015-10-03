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

    [TestFixture, Description("Income Received of Ordinary share - single parcel")]
    public class IncomeReceviedOrdinaryShareSingleParcel : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();

 /*           var testPortfolio = CreateTestPortfolio();

            var openingbalance = new OpeningBalance()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA",
                Units = 1000,
                CostBase = 1500.00m,
                Comment = "Test Opening Balance"
            };
            testPortfolio.Transactions.Add(openingbalance);

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2001, 01, 01),
                ASXCode = "AAA",
                FrankedAmount = 100.00m,
                UnfrankedAmount = 20.00m,
                FrankingCredits = 30.00m,
                Interest = 0.00m,
                TaxDeferred = 0.00m,
                Comment = "Income test"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            // Check that parcels are unchanged
            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);
            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(openingbalance.TransactionDate, _AAAId, openingbalance.Units, 1.50m, openingbalance.CostBase, openingbalance.CostBase, ParcelEvent.OpeningBalance)
            };
            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));

            // Check income received
            var actualIncome = testPortfolio.GetIncomeReceived(DateTimeConstants.NoStartDate(), DateTimeConstants.NoEndDate());
            var expectedIncome = new IncomeReceived[]
            {
                incomeReceived
            };
            Assert.That(actualIncome, PortfolioConstraint.Equals(expectedIncome)); */
        }
    }

    [TestFixture, Description("Income Received of Ordinary share - single parcel with DRP")]
    public class IncomeReceviedOrdinaryShareSingleParcelWithDRP : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Income Received of Trust- single parcel tax deferred")]
    public class IncomeReceviedTrustSingleParcelTaxDeferred : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Income Received of Ordinary share - mulitple parcels")]
    public class IncomeReceviedOrdinaryShareMultipleParcels : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Income Received of Ordinary share - mulitple parcels with DRP")]
    public class IncomeReceviedOrdinaryShareMultipleParcelsWithDRP : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Income Received of Trust - single parcel tax deferred greater than cost base")]
    public class IncomeReceviedTrustSingleParcelTaxDeferredGreaterThanCostbase : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            throw new NotSupportedException();
        }
    }

    [TestFixture, Description("Income Received validation tests")]
    public class IncomeReceviedValidationTests : TransactionTest
    {
        [Test, Description("Income Received Ordinary share - no parcels")]
        [ExpectedException(typeof(NoParcelsForTransaction))]
        public void NoParcelsForTransaction()
        {
            var transactions = new ITransaction[]
            {           
                new IncomeReceived()
                {
                    TransactionDate = new DateTime(2000, 01, 01),
                    ASXCode = "AAA"
                }
            };
            _Portfolio.Transactions.Add(transactions);
        }

        [Test, Description("Income Received of Stapled Security")]
        [ExpectedException(typeof(TransctionNotSupportedForStapledSecurity))]
        public void TransctionNotSupportedForStapledSecurity()
        {
            throw new NotSupportedException();
        }

    }

}
