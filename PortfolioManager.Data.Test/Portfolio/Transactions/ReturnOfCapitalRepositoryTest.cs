using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Test.Portfolio.Transactions
{
    [TestFixture]
    public class ReturnOfCapitalRepositoryTest : TransactionRepositoryTest
    {
        [Test, Description("Test adding a ReturnofCapital")]
        public void Add()
        {
            ReturnOfCapital returnOfCapital = new ReturnOfCapital()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Amount = 100.00M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""                
            };
            TestAdd(returnOfCapital, new ReturnOfCapitalComparer());
        }

        [Test, Description("Test updating a ReturnofCapital")]
        public void Update()
        {
            ReturnOfCapital returnOfCapital = new ReturnOfCapital()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Amount = 100.00M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestUpdate(returnOfCapital, new ReturnOfCapitalComparer());
        }

        [Test, Description("Test deleting a ReturnofCapital")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            ReturnOfCapital returnOfCapital = new ReturnOfCapital()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Amount = 100.00M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestDelete(returnOfCapital);
        }

        [Test, Description("Test deleting a ReturnofCapital")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            ReturnOfCapital returnOfCapital = new ReturnOfCapital()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Amount = 100.00M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestDeleteById(returnOfCapital);
        }

        protected override void UpdateTransaction(Transaction transaction)
        {
            ReturnOfCapital returnOfCapital = transaction as ReturnOfCapital;

            returnOfCapital.TransactionDate = new DateTime(2010, 04, 30);
            returnOfCapital.Amount = 150.00M;
            returnOfCapital.RecordDate = new DateTime(2010, 04, 30);
        }
    }
}
