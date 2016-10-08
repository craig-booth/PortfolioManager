using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PortfolioManager.Service;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service
{

    public class TransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly Dictionary<TransactionType, ITransactionHandler> _TransactionHandlers;
        private readonly AttachmentService _AttachmentService;

        internal event PortfolioChangedEventHandler PortfolioChanged;
        protected void OnPortfolioChanged()
        {
            PortfolioChanged?.Invoke(new PortfolioChangedEventArgs());
        }

        internal TransactionService(IPortfolioDatabase portfolioDatabase, ParcelService parcelService, StockService stockService, AttachmentService attachmentService)
        {
            _PortfolioDatabase = portfolioDatabase;
            _AttachmentService = attachmentService;

            _TransactionHandlers = new Dictionary<TransactionType, ITransactionHandler>();

            /* Add transaction handlers */
            _TransactionHandlers.Add(TransactionType.Aquisition, new AquisitionHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.CostBaseAdjustment, new CostBaseAdjustmentHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.Disposal, new DisposalHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.Income, new IncomeReceivedHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.OpeningBalance, new OpeningBalanceHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.ReturnOfCapital, new ReturnOfCapitalHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.UnitCountAdjustment, new UnitCountAdjustmentHandler(parcelService, stockService));
            _TransactionHandlers.Add(TransactionType.CashTransaction, new CashTransactionHandler());
        }

        public void ProcessTransaction(Transaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                ApplyTransaction(unitOfWork, transaction);                
                AddTransaction(unitOfWork, transaction);

                unitOfWork.Save();
            }

            OnPortfolioChanged();
        }

        public void ProcessTransactions(IEnumerable<Transaction> transactions)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (var transaction in transactions)
                {
                    ApplyTransaction(unitOfWork, transaction);                
                    AddTransaction(unitOfWork, transaction);

                };
                unitOfWork.Save();
            }

            OnPortfolioChanged();
        }

        private void AddTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            unitOfWork.TransactionRepository.Add(transaction);
        }

        internal void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var handler = _TransactionHandlers[transaction.Type];
            if (handler != null)
            {
                handler.ApplyTransaction(unitOfWork, transaction);
            }
            else
                throw new NotSupportedException("Transaction type not supported");
        }

        public IReadOnlyCollection<Transaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(fromDate, toDate);
        }

        public IReadOnlyCollection<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(asxCode, fromDate, toDate);
        }

        public IReadOnlyCollection<Transaction> GetTransactions(string asxCode, TransactionType type, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(asxCode, type, fromDate, toDate);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);

            OnPortfolioChanged();
        }

        public void DeleteTransaction(Transaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Delete(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);

            OnPortfolioChanged();
        }

    }
}
