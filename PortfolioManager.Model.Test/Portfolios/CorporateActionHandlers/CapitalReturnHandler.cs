using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;


namespace PortfolioManager.Model.Test.Portfolios.CorporateActionHandlers
{
    [TestFixture, Description("Capital Return Handler Tests")]
    public class CapitalReturnHandlerTest : PortfolioTest
    {
        [Test, Description("Single Parcel")]
        public void SingleParcel()
        {
            var purchaseDate = new DateTime(2000, 01, 01);
            var recordDate = new DateTime(2010, 01, 01);
            var paymentDate = new DateTime(2010, 02, 01);

            var transactions = new ITransaction[]
            {
                new Aquisition()
                {
                    TransactionDate = purchaseDate,
                    ASXCode = "AAA",
                    Units = 1000,
                    AveragePrice = 10.00m,
                    TransactionCosts = 19.95m,
                    Comment = "Test aquisition"
                }
            };
            _Portfolio.TransactionService.ProcessTransactions(transactions);


            var capitalReturn = new CapitalReturn(_StockDatabase, _StockManager.GetStock("AAA", recordDate).Id, recordDate, paymentDate, 0.50m, "Test");

            var actualTransactions = _Portfolio.CorporateActionService.CreateTransactionList(capitalReturn);

            var expectedTransactions = new ITransaction[]
            {
                new ReturnOfCapital()
                {
                    TransactionDate = paymentDate,
                    ASXCode = "AAA",
                    RecordDate = recordDate,
                    Amount = 1000 * 0.50m,
                }
            };

            Assert.That(actualTransactions, PortfolioConstraint.Equals(expectedTransactions));
        }

        public void MultipleParcels()
        {

        }

        public void NoParcels()
        {

        }

    }

}
