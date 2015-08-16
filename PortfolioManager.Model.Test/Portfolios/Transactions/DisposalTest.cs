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
    public class DisposalTest : PortfolioTestBase
    {

        [Test, Description("Ordinary - single parcel, sell all")]
        public void OrdinaryShareSingleParcelSellAll()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Ordinary - single parcel, sell part")]
        public void OrdinaryShareSingleParcelSellPart()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Ordinary - multiple parcels, sell all")]
        public void OrdinaryShareMultipleParcelsSellAll()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Ordinary - multiple parcels, sell part")]
        public void OrdinaryShareMultipleParcelsSellPart()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Ordinary - no parcels")]
        public void OrdinaryShareNoParcels()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Ordinary - single parcel, not enough shares")]
        public void OrdinaryShareSingleParcelNotEnoughShares()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Ordinary - multiple parcels, not enough shares")]
        public void OrdinaryShareMultipleParcelsNotEnoughShares()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Stapled - single parcel, sell part")]
        public void StapledSingleParcelSellPart()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Stapled - multiple parcels, sell all")]
        public void StapledMultipleParcelsSellAll()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Stapled - multiple parcels, sell part")]
        public void StapledMultipleParcelsSellPart()
        {
            var testPortfolio = CreateTestPortfolio();

            var disposal = new Disposal()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "AAA"
            };
            testPortfolio.Transactions.Add(disposal);

            var actualParcels = testPortfolio.GetParcels(disposal.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

    }
}
