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
    public class AquisitionTest : PortfolioTestBase
    {

        [Test, Description("Purchase stock")]
        public void PurchaseStock()
        {
            var testPortfolio = CreateTestPortfolio();

            var aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            testPortfolio.Transactions.Add(aquisition);
           
            var actualParcels = testPortfolio.GetParcels(aquisition.TransactionDate);

            decimal costBase = 10019.95m; // (1000 * 10) + 19.95
            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(aquisition.TransactionDate, _AAAId, aquisition.Units, aquisition.AveragePrice, costBase, costBase, ParcelEvent.Aquisition)
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Purchase stapled security")]
        public void PurchaseStapledSecurity()
        {
            var testPortfolio = CreateTestPortfolio();

            var aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "SSS",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            testPortfolio.Transactions.Add(aquisition);

            var actualParcels = testPortfolio.GetParcels(aquisition.TransactionDate).ToList(); 
            // Add in stapled security
            actualParcels.AddRange(testPortfolio.GetParcels(_SSSId, aquisition.TransactionDate));

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var mainParcel = new ShareParcel(aquisition.TransactionDate, _SSSId, aquisition.Units, 10.00m, 10019.95m, 10019.95m, ParcelEvent.Aquisition);
            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(aquisition.TransactionDate, _SSS1Id, aquisition.Units, 1.00m, 1002.00m, 1002.00m, mainParcel.Id, ParcelEvent.Aquisition),
                new ShareParcel(aquisition.TransactionDate, _SSS2Id, aquisition.Units, 3.00m, 3005.98m, 3005.98m, mainParcel.Id, ParcelEvent.Aquisition),
                new ShareParcel(aquisition.TransactionDate, _SSS3Id, aquisition.Units, 6.00m, 6011.97m, 6011.97m, mainParcel.Id, ParcelEvent.Aquisition),   
                mainParcel
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));    
        }
    }

    [TestFixture]
    public class AquisitionOrdinaryShare : PortfolioTestBase2
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            var aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            _Portfolio.Transactions.Add(aquisition);
        }

        [Test]
        public void ExpectedParcels()
        {
            var actualParcels = _Portfolio.GetParcels(new DateTime(2000, 01, 01));

            decimal costBase = 10019.95m; // (1000 * 10) + 19.95
            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(new DateTime(2000, 01, 01), _AAAId, 1000, 10.00m, costBase, costBase, ParcelEvent.Aquisition)
            };

            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test]
        public void ExpectedIncome()
        {
            var actualIncome = _Portfolio.GetIncomeReceived(DateTimeConstants.NoStartDate(), DateTimeConstants.NoEndDate());

            Assert.That(actualIncome, Is.Empty);
        }

        [Test]
        public void ExpectedCGTEvents()
        {
            var actualCGTEvents = _Portfolio.GetCGTEvents(DateTimeConstants.NoStartDate(), DateTimeConstants.NoEndDate());

            Assert.That(actualCGTEvents, Is.Empty);
        }
    }
}
