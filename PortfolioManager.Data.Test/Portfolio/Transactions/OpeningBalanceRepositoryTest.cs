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
    public class OpeningBalanceRepositoryTest : TransactionRepositoryTest
    {
        [Test, Description("Test adding a OpeningBalance")]
        public void Add()
        {
            OpeningBalance openingBalance = new OpeningBalance()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                CostBase = 1230.79M,
                AquisitionDate = new DateTime(2010, 10, 04),
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestAdd(openingBalance, new OpeningBalanceComparer());
        }

        [Test, Description("Test updating a OpeningBalance")]
        public void Update()
        {
            OpeningBalance openingBalance = new OpeningBalance()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                CostBase = 1230.79M,
                AquisitionDate = new DateTime(2010, 10, 04),
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestUpdate(openingBalance, new OpeningBalanceComparer());
        }

        [Test, Description("Test deleting a OpeningBalance")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            OpeningBalance openingBalance = new OpeningBalance()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                CostBase = 1230.79M,
                AquisitionDate = new DateTime(2010, 10, 04),
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestDelete(openingBalance);
        }

        [Test, Description("Test deleting a OpeningBalance")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            OpeningBalance openingBalance = new OpeningBalance()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                CostBase = 1230.79M,
                AquisitionDate = new DateTime(2010, 10, 04),
                RecordDate = new DateTime(2010, 10, 04),
                Comment = ""
            };
            TestDeleteById(openingBalance);
        }

        protected override void UpdateTransaction(Transaction transaction)
        {
            OpeningBalance openingBalance = transaction as OpeningBalance;

            openingBalance.TransactionDate = new DateTime(2010, 04, 30);
            openingBalance.Units = 150;
            openingBalance.CostBase = 2230.49M;
            openingBalance.AquisitionDate = new DateTime(2010, 04, 15);
            openingBalance.RecordDate = new DateTime(2010, 04, 30);
        }
    }
}

