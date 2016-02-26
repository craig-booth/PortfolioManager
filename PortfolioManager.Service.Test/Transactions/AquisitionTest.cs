using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Test.Transactions
{

    [TestFixture, Description("Purchase stock")]
    public class AquisitionOrdinaryShare : TransactionTestWithExpectedTests 
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var aquisition = new Aquisition()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            _Portfolio.TransactionService.ProcessTransaction(aquisition);

            decimal costBase = 10019.95m; // (1000 * 10) + 19.95
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, aquisition.Units, aquisition.AveragePrice, costBase, costBase, ParcelEvent.Aquisition));
        }
    }

    [TestFixture, Description("Purchase stapled security")]
    public class AquisitionStapledSecurity : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var aquisition = new Aquisition()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "SSS",
                Units = 1000,
                AveragePrice = 10.00m,
                TransactionCosts = 19.95m,
                Comment = "Test aquisition"
            };
            _Portfolio.TransactionService.ProcessTransaction(aquisition);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, aquisition.Units, 1.00m, 1002.00m, 1002.00m, purchaseId, ParcelEvent.Aquisition));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, aquisition.Units, 3.00m, 3005.98m, 3005.98m, purchaseId, ParcelEvent.Aquisition));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, aquisition.Units, 6.00m, 6011.97m, 6011.97m, purchaseId, ParcelEvent.Aquisition));
        }
    }

    [TestFixture, Description("Aquisition validation tests")]
    public class AquisitionValidationTests : PortfolioTest
    {
        [Test, Description("Aquire child security")]
        [ExpectedException(typeof(TransctionNotSupportedForChildSecurity))]
        public void NotSupportedForChildSecurity()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var transactions = new ITransaction[]
            {
                new Aquisition()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "SSS1",
                    Units = 1000,
                    AveragePrice = 1.70m,
                    TransactionCosts = 10.00m,
                    Comment = ""
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }
    }

}
