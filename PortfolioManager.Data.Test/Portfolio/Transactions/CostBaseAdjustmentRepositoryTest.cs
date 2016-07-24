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
    public class CostBaseAdjustmentRepositoryTest : TransactionRepositoryTest
    {
        [Test, Description("Test adding a CostBaseAdjustment")]
        public void Add()
        {
            CostBaseAdjustment costbaseAdjustment = new CostBaseAdjustment()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Percentage = 0.40M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test cost base"
            };
            TestAdd(costbaseAdjustment, new CostBaseAdjustmentComparer());
        }

        [Test, Description("Test updating a CostBaseAdjustment")]
        public void Update()
        {
            CostBaseAdjustment costbaseAdjustment = new CostBaseAdjustment()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Percentage = 0.40M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test cost base"
            };
            TestUpdate(costbaseAdjustment, new CostBaseAdjustmentComparer());
        }

        [Test, Description("Test deleting a CostBaseAdjustment")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            CostBaseAdjustment costbaseAdjustment = new CostBaseAdjustment()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Percentage = 0.40M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test cost base"
            };
            TestDelete(costbaseAdjustment);
        }

        [Test, Description("Test deleting a CostBaseAdjustment by id")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            CostBaseAdjustment costbaseAdjustment = new CostBaseAdjustment()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Percentage = 0.40M,
                RecordDate = new DateTime(2010, 10, 04),
                Comment = "Test cost base"
            };
            TestDeleteById(costbaseAdjustment);
        }

        protected override void UpdateTransaction(Transaction transaction)
        {
            CostBaseAdjustment costbaseAdjustment = transaction as CostBaseAdjustment;

            costbaseAdjustment.TransactionDate = new DateTime(2010, 04, 30);
            costbaseAdjustment.Percentage = 0.30M;
        }
    }
}