using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service.Local
{
    class TransactionService : ITransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;

        public TransactionService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;
        }

        public Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var transaction = Mapper.Map<Transaction>(transactionItem);

                    var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

                    var handler = transactionHandlerFactory.GetHandler(transaction);
                    if (handler != null)
                    {
                        handler.ApplyTransaction(portfolioUnitOfWork, transaction);
                        portfolioUnitOfWork.TransactionRepository.Add(transaction);
                    }
                    else
                    {
                        throw new NotSupportedException("Transaction type not supported");
                    }

                    portfolioUnitOfWork.Save();

                    responce.SetStatusToSuccessfull();
                }

            }

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

                    var transactions = Mapper.Map<List<Transaction>>(transactionItems);

                    foreach (var transaction in transactions)
                    {
                        var handler = transactionHandlerFactory.GetHandler(transaction);
                        if (handler != null)
                        {
                            handler.ApplyTransaction(portfolioUnitOfWork, transaction);
                            portfolioUnitOfWork.TransactionRepository.Add(transaction);
                        }
                        else
                        {
                            throw new NotSupportedException("Transaction type not supported");
                        }                     

                    };

                    portfolioUnitOfWork.Save();

                    responce.SetStatusToSuccessfull();
                }

            }

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> DeleteTransaction(Guid id)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                portfolioUnitOfWork.TransactionRepository.Delete(id);
                portfolioUnitOfWork.Save();

                responce.SetStatusToSuccessfull();               
            }

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

        public Task<GetTransactionResponce> GetTransaction(Guid id)
        {
            var responce = new GetTransactionResponce();

            using (IPortfolioReadOnlyUnitOfWork unitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                responce.Transaction = Mapper.Map<TransactionItem>(unitOfWork.PortfolioQuery.GetTransaction(id));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<GetTransactionResponce>(responce);
        }

        public Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate)
        {       
            var responce = new GetTransactionsResponce();

            using (IPortfolioReadOnlyUnitOfWork unitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var transactions = unitOfWork.PortfolioQuery.GetTransactions(fromDate, toDate);
                responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<GetTransactionsResponce>(responce); 
        }

        public Task<GetTransactionsResponce> GetTransactions(Guid stockId, DateTime fromDate, DateTime toDate)
        {
            var responce = new GetTransactionsResponce();

            using (IPortfolioReadOnlyUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (IStockReadOnlyUnitOfWork stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var asxCode = stockUnitOfWork.StockQuery.GetASXCode(stockId, fromDate);

                    var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(asxCode, fromDate, toDate);
                    responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

    }

    class TransactionHandlerFactory
    {
        private readonly ServiceFactory<ITransactionHandler> _TransactionHandlers;
   
        public TransactionHandlerFactory(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _TransactionHandlers = new ServiceFactory<ITransactionHandler>();
            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<Disposal>(() => new DisposalHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<IncomeReceived>(() => new IncomeReceivedHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<OpeningBalance>(() => new OpeningBalanceHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(portfolioQuery, stockQuery));
            _TransactionHandlers.Register<CashTransaction>(() => new CashTransactionHandler());
        }

        public ITransactionHandler GetHandler(Transaction transaction)
        {
            return _TransactionHandlers.GetService(transaction);
        }
    }
}
