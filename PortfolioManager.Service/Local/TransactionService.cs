using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service.Local
{
    class TransactionService : ITransactionService
    {
        private readonly ServiceFactory<ITransactionHandler> _TransactionHandlers;
        private readonly IPortfolioDatabase _PortfolioDatabase;

        private readonly IPortfolioQuery _PortfolioQuery;
        private readonly IStockQuery _StockQuery;

        public TransactionService(IPortfolioDatabase portfolioDatabase, IStockQuery stockQuery)
        {
            _PortfolioDatabase = portfolioDatabase;
            _PortfolioQuery = portfolioDatabase.PortfolioQuery;
            _StockQuery = stockQuery;

            _TransactionHandlers = new ServiceFactory<Transactions.ITransactionHandler>();
            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<Disposal>(() => new DisposalHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<IncomeReceived>(() => new IncomeReceivedHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<OpeningBalance>(() => new OpeningBalanceHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(_PortfolioQuery, stockQuery));
            _TransactionHandlers.Register<CashTransaction>(() => new CashTransactionHandler());
        }

        /* TODO: This is temporary */
        public void LoadTransaction(IPortfolioUnitOfWork unitOfWork, Transaction transaction)
        {
            var handler = _TransactionHandlers.GetService(transaction);
            if (handler != null)
            {
                handler.ApplyTransaction(unitOfWork, transaction);
            }
            else
            {
                throw new NotSupportedException("Transaction type not supported");
            }
        }

        public Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                var handler = _TransactionHandlers.GetService(transaction);
                if (handler != null)
                {
                    handler.ApplyTransaction(unitOfWork, transaction);
                    unitOfWork.TransactionRepository.Add(transaction);
                }
                else
                {
                    throw new NotSupportedException("Transaction type not supported"); 
                }

                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            var responce = new ServiceResponce();

            var transactions = Mapper.Map<List<Transaction>>(transactionItems);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                
                foreach (var transaction in transactions)
                {
                    var handler = _TransactionHandlers.GetService(transaction);
                    if (handler != null)
                    {
                        handler.ApplyTransaction(unitOfWork, transaction);
                        unitOfWork.TransactionRepository.Add(transaction);
                    }
                    else
                    {
                        throw new NotSupportedException("Transaction type not supported");
                    }

                    handler.ApplyTransaction(unitOfWork, transaction);
                    unitOfWork.TransactionRepository.Add(transaction);

                };
                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> DeleteTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Delete(transaction);
                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> UpdateTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate)
        {       
            var responce = new GetTransactionsResponce();

            var transactions = _PortfolioDatabase.PortfolioQuery.GetTransactions(fromDate, toDate);
            responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));
            
            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce); 
        }

        public Task<GetTransactionsResponce> GetTransactions(Guid stockId, DateTime fromDate, DateTime toDate)
        {
            var responce = new GetTransactionsResponce();

            var asxCode = _StockQuery.GetASXCode(stockId, fromDate);

            var transactions = _PortfolioQuery.GetTransactions(asxCode, fromDate, toDate);
            responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

    }
}
