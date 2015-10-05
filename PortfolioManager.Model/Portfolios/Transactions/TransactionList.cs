using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class TransactionList
    {
        private Portfolio _Portfolio;
        private IPortfolioDatabase _PortfolioDatabase;

        public TransactionList(IPortfolioDatabase database, Portfolio portfolio)
        {
            _Portfolio = portfolio;
            _PortfolioDatabase = database;
        }


        public void Add(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                _Portfolio.ValidateTransaction(transaction);
                unitOfWork.TransactionRepository.Add(transaction);
                unitOfWork.Save();
            }

            _Portfolio.ApplyTransaction(transaction);
        }

        public void Add(IEnumerable<ITransaction> transactions)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (ITransaction transaction in transactions)
                {
                    _Portfolio.ValidateTransaction(transaction);
                    unitOfWork.TransactionRepository.Add(transaction);
                    _Portfolio.ApplyTransaction(transaction);
                };
                unitOfWork.Save();
            }
        }


        public void Update(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);
        }

        public void Delete(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Delete(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
           // _Portfolio.ApplyTransaction(transaction);
        }

        public IReadOnlyCollection<ITransaction> Find(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(_Portfolio.Id, fromDate, toDate);
        }

        public IReadOnlyCollection<ITransaction> Find(string asxCode, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(_Portfolio.Id, asxCode, fromDate, toDate);
        }
        
    }
}
