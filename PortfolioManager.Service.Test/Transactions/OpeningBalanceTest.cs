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

    [TestFixture, Description("Opening balance for ordinary share")]
    public class OpeningBalanceOrdinaryShare : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var openingbalance = new OpeningBalance()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "AAA",
                Units = 1000,
                CostBase = 1500.00m,
                AquisitionDate = _TransactionDate,
                Comment = "Test Opening Balance"
            };
            _Portfolio.TransactionService.ProcessTransaction(openingbalance);

            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("AAA", _TransactionDate).Id, openingbalance.Units, 1.50m, openingbalance.CostBase, openingbalance.CostBase, ParcelEvent.OpeningBalance));
        }
    }

    [TestFixture, Description("Opening balance for stapled security")]
    public class OpeningBalanceStapledSecurity : TransactionTestWithExpectedTests
    {
        public override void PerformTest()
        {
            _TransactionDate = new DateTime(2000, 01, 01);

            var openingbalance = new OpeningBalance()
            {
                TransactionDate = _TransactionDate,
                ASXCode = "SSS",
                Units = 1000,
                CostBase = 15000.00m,
                AquisitionDate = _TransactionDate,
                Comment = "Test Opening Balance"
            };
            _Portfolio.TransactionService.ProcessTransaction(openingbalance);

            // Relative NTA... s1 = 10% ,s2 = 30%, s3 = 60%
            var purchaseId = Guid.NewGuid();
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("SSS1", _TransactionDate).Id, openingbalance.Units, 1.50m, 1500.00m, 1500.00m, purchaseId, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("SSS2", _TransactionDate).Id, openingbalance.Units, 4.50m, 4500.00m, 4500.00m, purchaseId, ParcelEvent.OpeningBalance));
            _ExpectedParcels.Add(new ShareParcel(_TransactionDate, _StockServiceRepository.StockService.GetStock("SSS3", _TransactionDate).Id, openingbalance.Units, 9.00m, 9000.00m, 9000.00m, purchaseId, ParcelEvent.OpeningBalance));
        }
     }

    [TestFixture, Description("Opening balance validation tests")]
    public class OpeningBalanceValidationTests : PortfolioTest
    {
        [Test, Description("Opening balance for child security")]
        [ExpectedException(typeof(TransctionNotSupportedForChildSecurity))]
        public void NotSupportedForChildSecurity()
        {
            var transactionDate = new DateTime(2002, 01, 01);

            var transactions = new ITransaction[]
            {
                new OpeningBalance()
                {
                    TransactionDate = transactionDate,
                    ASXCode = "SSS1",
                    Units = 1000,
                    CostBase = 15000.00m,
                    AquisitionDate = transactionDate,
                    Comment = "Test Opening Balance"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);
        }
    }

}
