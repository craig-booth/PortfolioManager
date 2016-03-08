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
    public class TransactionRepositoryTest : TestBase 
    {
        public void TestAdd(Transaction transaction, TransactionComparer comparer)
        {
            Transaction transaction2;

            var database = CreatePortfolioDatabase();
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(transaction);
            
                transaction2 = unitOfWork.TransactionRepository.Get(transaction.Id);
                Assert.That(transaction, Is.EqualTo(transaction2).Using(comparer));
            }
        }

        protected virtual void UpdateTransaction(Transaction transaction)
        {

        }

        public void TestUpdate(Transaction transaction, TransactionComparer comparer)
        {
            Transaction transaction2, transaction3;
      
            var database = CreatePortfolioDatabase();
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(transaction);

                transaction2 = unitOfWork.TransactionRepository.Get(transaction.Id);
                UpdateTransaction(transaction2);
                unitOfWork.TransactionRepository.Update(transaction2);

                transaction3 = unitOfWork.TransactionRepository.Get(transaction.Id);
                Assert.That(transaction2, Is.EqualTo(transaction3).Using(comparer));
            }
        }

        public void TestDelete(Transaction transaction)
        {
            Transaction transaction2, transaction3;

            var database = CreatePortfolioDatabase();
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(transaction);

                transaction2 = unitOfWork.TransactionRepository.Get(transaction.Id);

                unitOfWork.TransactionRepository.Delete(transaction2);

                transaction3 = unitOfWork.TransactionRepository.Get(transaction.Id);
            }
        }


        public void TestDeleteById(Transaction transaction)
        {
            Transaction transaction2, transaction3;

            var database = CreatePortfolioDatabase();
            using (IPortfolioUnitOfWork unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(transaction);

                transaction2 = unitOfWork.TransactionRepository.Get(transaction.Id);

                unitOfWork.TransactionRepository.Delete(transaction2.Id);

                transaction3 = unitOfWork.TransactionRepository.Get(transaction.Id);
            }
        }
    }
}
