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
    public class IncomeReceivedRepositoryTest : TransactionRepositoryTest
    {
        [Test, Description("Test adding a IncomeReceived")]
        public void Add()
        {
            IncomeReceived incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2010, 10, 15),
                ASXCode = "ABC",
                RecordDate = new DateTime(2010, 10, 04),
                FrankedAmount = 1.20M,
                UnfrankedAmount = 0.80M,
                FrankingCredits = 0.20M,
                Interest = 0.10M,
                TaxDeferred = 0.88M,
                Comment = ""
            };
            TestAdd(incomeReceived, new IncomeReceivedComparer());
        }

        [Test, Description("Test updating a IncomeReceived")]
        public void Update()
        {
            IncomeReceived incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2010, 10, 15),
                ASXCode = "ABC",
                RecordDate = new DateTime(2010, 10, 04),
                FrankedAmount = 1.20M,
                UnfrankedAmount = 0.80M,
                FrankingCredits = 0.20M,
                Interest = 0.10M,
                TaxDeferred = 0.88M,
                Comment = ""
            };
            TestUpdate(incomeReceived, new IncomeReceivedComparer());
        }

        [Test, Description("Test deleting a IncomeReceived")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void Delete()
        {
            IncomeReceived incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2010, 10, 15),
                ASXCode = "ABC",
                RecordDate = new DateTime(2010, 10, 04),
                FrankedAmount = 1.20M,
                UnfrankedAmount = 0.80M,
                FrankingCredits = 0.20M,
                Interest = 0.10M,
                TaxDeferred = 0.88M,
                Comment = ""
            };
            TestDelete(incomeReceived);
        }

        [Test, Description("Test deleting a IncomeReceived")]
        [ExpectedException(typeof(RecordNotFoundException))]
        public void DeleteById()
        {
            IncomeReceived incomeReceived = new IncomeReceived()
            {
                TransactionDate = new DateTime(2010, 10, 15),
                ASXCode = "ABC",
                RecordDate = new DateTime(2010, 10, 04),
                FrankedAmount = 1.20M,
                UnfrankedAmount = 0.80M,
                FrankingCredits = 0.20M,
                Interest = 0.10M,
                TaxDeferred = 0.88M,
                Comment = ""
            };
            TestDeleteById(incomeReceived);
        }

        protected override void UpdateTransaction(ITransaction transaction)
        {
            IncomeReceived incomeReceived = transaction as IncomeReceived;

            incomeReceived.TransactionDate = new DateTime(2010, 04, 30);
            incomeReceived.RecordDate = new DateTime(2010, 05, 10);
            incomeReceived.FrankedAmount = 1.50M;
            incomeReceived.UnfrankedAmount = 0.90M;
            incomeReceived.FrankingCredits = 1.20M;
            incomeReceived.Interest = 1.10M;
            incomeReceived.TaxDeferred = 1.88M;
        }
    }
}

