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
    public class AquisitionRepositoryTest : TransactionRepositoryTest
    {
        [Test, Description("Test adding an Aquisition")]
        public void Add()
        {
            Aquisition aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                Comment = "Test add"
            };
            TestAdd(aquisition, new AquisitionComparer());
        }

        [Test, Description("Test updating an Aquisition")]
        public void Update()
        {
            Aquisition aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                Comment = "Test add"
            };
            TestUpdate(aquisition, new AquisitionComparer()); 
        }

        [Test, Description("Test deleting an Aquisition")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            Aquisition aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                Comment = "Test add"
            };
            TestDelete(aquisition); 
        }

        [Test, Description("Test deleting an Aquisition by id")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            Aquisition aquisition = new Aquisition()
            {
                TransactionDate = new DateTime(2010, 10, 04),
                ASXCode = "ABC",
                Units = 100,
                AveragePrice = 5.55M,
                TransactionCosts = 19.95M,
                Comment = "Test add"
            };
            TestDeleteById(aquisition); 
        }

        protected override void UpdateTransaction(ITransaction transaction)
        {
            Aquisition aquisition = transaction as Aquisition;

            aquisition.TransactionDate = new DateTime(2010, 04, 30);
            aquisition.Units = 150;
        }
    }
}
