using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class TransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly Dictionary<TransactionType, ITransactionHandler> _TransactionHandlers;


        internal TransactionService(IPortfolioDatabase portfolioDatabase, ParcelService parcelService, StockService stockService)
        {
            _PortfolioDatabase = portfolioDatabase;

            _TransactionHandlers = new Dictionary<TransactionType, ITransactionHandler>();

            /* Add transaction handlers */
            _TransactionHandlers.Add(TransactionType.Aquisition, new AquisitionHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.CostBaseAdjustment, new CostBaseAdjustmentHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.Disposal, new DisposalHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.Income, new IncomeReceivedHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.OpeningBalance, new OpeningBalanceHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.ReturnOfCapital, new ReturnOfCapitalHandler(parcelService, stockService));
            var cashTransactionHandler = new CashTransactionHandler();
            _TransactionHandlers.Add(TransactionType.Deposit, cashTransactionHandler);
            _TransactionHandlers.Add(TransactionType.Withdrawl, cashTransactionHandler);
            _TransactionHandlers.Add(TransactionType.Fee, cashTransactionHandler);
            _TransactionHandlers.Add(TransactionType.Interest, cashTransactionHandler);
        }

        public void ProcessTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                ApplyTransaction(unitOfWork, transaction);
                unitOfWork.TransactionRepository.Add(transaction);
                unitOfWork.Save();
            }
        }

        public void ProcessTransactions(IEnumerable<ITransaction> transactions)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (ITransaction transaction in transactions)
                {
                    ApplyTransaction(unitOfWork, transaction);
                    unitOfWork.TransactionRepository.Add(transaction);

                };
                unitOfWork.Save();
            }
        }

        internal void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            var handler = _TransactionHandlers[transaction.Type];
            if (handler != null)
            {
                handler.ApplyTransaction(unitOfWork, transaction);
            }
            else
                throw new NotSupportedException("Transaction type not supported");
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Guid.Empty, fromDate, toDate);
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Guid.Empty, asxCode, fromDate, toDate);
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(string asxCode, TransactionType type, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Guid.Empty, asxCode, type, fromDate, toDate);
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);
        }

        public void DeleteTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Delete(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);
        }
    }
}
