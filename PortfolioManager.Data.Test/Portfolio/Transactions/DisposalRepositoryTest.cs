using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Test.Portfolio.Transactions
{
    [TestFixture]
    public class DisposalRepositoryTest : TransactionRepositoryTest
    {
        [Test, Description("Test adding a Disposal")]
        public void Add()
        {
            Disposal disposal = new Disposal()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                CGTMethod = CGTCalculationMethod.MinimizeGain,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test disposal"
            };
            TestAdd(disposal, new DisposalComparer());
        }

        [Test, Description("Test updating a Disposal")]
        public void Update()
        {
            Disposal disposal = new Disposal()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                CGTMethod = CGTCalculationMethod.MinimizeGain,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test disposal"
            };
            TestUpdate(disposal, new DisposalComparer());
        }

        [Test, Description("Test deleting a Disposal")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            Disposal disposal = new Disposal()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                CGTMethod = CGTCalculationMethod.MinimizeGain,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test disposal"
            };
            TestDelete(disposal);
        }

        [Test, Description("Test deleting a Disposal")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            Disposal disposal = new Disposal()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                CGTMethod = CGTCalculationMethod.MinimizeGain,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test disposal"
            };
            TestDeleteById(disposal);
        }

        protected override void UpdateTransaction(Transaction transaction)
        {
            Disposal disposal = transaction as Disposal;

            disposal.TransactionDate = new DateTime(2010, 04, 30);
            disposal.Units = 150;
        }
    }
}

