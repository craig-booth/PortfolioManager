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
    [TestFixture]
    public class IncomeReceivedTest : PortfolioTestBase
    {
        [Test, Description("Income Received of Ordinary share - single parcel")]
        public void OrdinaryShareSingleParcel()
        {
            var testPortfolio = CreateTestPortfolio();

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
            Assert.That(actualIncome, PortfolioConstraint.Equals(expectedIncome));

        }
 
        [Test, Description("Income Received of Ordinary - no parcels")]
        public void OrdinaryShareNoParcels()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Income Received of Ordinary share - single parcel  with DRP")]
        public void OrdinaryShareSingleParcelWithDRP()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Income Received of Trust- single parcel tax deferred")]
        public void TrustSingleParcelTaxDeferred()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Income Received of Ordinary share - mulitple parcels")]
        public void OrdinaryShareMutlipleParcels()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Income Received of Ordinary share - mulitple parcels with DRP")]
        public void OrdinaryShareMultipleParcelsWithDRP()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Income Received of Trust - multiple parcels tax deferred")]
        public void TrustMultipleParcelsTaxDeferred()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }
        
        [Test, Description("Income Received of Trust - single parcel tax deferred greater than cost base")]
        public void TrustSingleParcelTaxDeferredGreaterThanCostbase()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Income Received of Stapled Security")]
        public void StapledSecuritySingleParcel()
        {
            var testPortfolio = CreateTestPortfolio();

            var incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(incomeReceived);

            var actualParcels = testPortfolio.GetParcels(incomeReceived.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

    }
}
