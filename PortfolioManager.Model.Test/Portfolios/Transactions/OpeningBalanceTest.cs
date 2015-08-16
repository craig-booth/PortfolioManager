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
    public class OpeningBalananceTest : PortfolioTestBase
    {

        [Test, Description("Opening balance for stock")]
        public void OpeningBalanceForStock()
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

            var actualParcels = testPortfolio.GetParcels(openingbalance.TransactionDate);

            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(openingbalance.TransactionDate, _AAAId, openingbalance.Units, 1.50m, openingbalance.CostBase, openingbalance.CostBase, ParcelEvent.OpeningBalance)
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));
        }

        [Test, Description("Opening balance for stapled security")]
        public void OpeningBalanceForStapledSecurity()
        {
            var testPortfolio = CreateTestPortfolio();

            var openingbalance = new OpeningBalance()
            {
                TransactionDate = new DateTime(2000, 01, 01),
                ASXCode = "SSS",
                Units = 1000,
                CostBase = 15000.00m,
                Comment = "Test Opening Balance"
            };
            testPortfolio.Transactions.Add(openingbalance);

            var actualParcels = testPortfolio.GetParcels(openingbalance.TransactionDate).ToList();
            // Add in stapled security
            actualParcels.AddRange(testPortfolio.GetParcels(_SSSId, openingbalance.TransactionDate));

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var mainParcel = new ShareParcel(openingbalance.TransactionDate, _SSSId, openingbalance.Units, 15.00m, 15000.00m, 15000.00m, ParcelEvent.OpeningBalance);
            var expectedParcels = new ShareParcel[]
            {
                new ShareParcel(openingbalance.TransactionDate, _SSS1Id, openingbalance.Units, 1.50m, 1500.00m, 1500.00m, mainParcel.Id, ParcelEvent.OpeningBalance),
                new ShareParcel(openingbalance.TransactionDate, _SSS2Id, openingbalance.Units, 4.50m, 4500.00m, 4500.00m, mainParcel.Id, ParcelEvent.OpeningBalance),
                new ShareParcel(openingbalance.TransactionDate, _SSS3Id, openingbalance.Units, 9.00m, 9000.00m, 9000.00m, mainParcel.Id, ParcelEvent.OpeningBalance),   
                mainParcel
            };


            Assert.That(actualParcels, PortfolioConstraint.Equals(expectedParcels));  
        }
    }
}
