using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service.Local
{
    public class TransactionService : ITransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;
        private readonly IMapper _Mapper;

        public TransactionService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase, IMapper mapper)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;
            _Mapper = mapper;
        }

        public Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    transactionItem.Id = Guid.NewGuid();
                    var transaction = _Mapper.Map<Transaction>(transactionItem);

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

                    foreach (var transactionItem in transactionItems)
                    {
                        transactionItem.Id = Guid.NewGuid();
                        var transaction = _Mapper.Map<Transaction>(transactionItem);             

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
                    }

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

            var transaction = _Mapper.Map<Transaction>(transactionItem);

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
                responce.Transaction = _Mapper.Map<TransactionItem>(unitOfWork.PortfolioQuery.GetTransaction(id));

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
                responce.Transactions.AddRange(_Mapper.Map<IEnumerable<TransactionItem>>(transactions));

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
                    responce.Transactions.AddRange(_Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

        public Task<ServiceResponce> ImportTransactions(string fileName)
        {
            var responce = new ServiceResponce();

            var importer = new TransactionImporter();
            var transactions = importer.ImportTransactions(fileName);

            ImportTransactions(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce);
        }

        public Task<ServiceResponce> ImportTransactions(TextReader textReader)
        {
            var responce = new ServiceResponce();

            var importer = new TransactionImporter();
            var transactions = importer.ImportTransactions(textReader);

            ImportTransactions(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<ServiceResponce>(responce);
        }

        private void ImportTransactions(IEnumerable<Transaction> transactions)
        {
            using (IPortfolioUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

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
                    }

                    portfolioUnitOfWork.Save();
                }
            }
        }      

        public Task<ServiceResponce> ExportTransactions(string fileName, DateTime fromDate, DateTime toDate)
        {
            var responce = new ServiceResponce();
    
            using (IPortfolioReadOnlyUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var exporter = new TransactionExporter();

                var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(fromDate, toDate);

                exporter.ExportTransactions(fileName, transactions);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<ServiceResponce>(responce);
        }

        public Task<ServiceResponce> ExportTransactions(TextWriter textWriter, DateTime fromDate, DateTime toDate)
        {
            var responce = new ServiceResponce();

            using (IPortfolioReadOnlyUnitOfWork portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var exporter = new TransactionExporter();

                var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(fromDate, toDate);

                exporter.ExportTransactions(textWriter, transactions);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<ServiceResponce>(responce);
        }
    }

    class TransactionHandlerFactory
    {
        private readonly ServiceFactory<ITransactionHandler> _TransactionHandlers;
   
        public TransactionHandlerFactory(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _TransactionHandlers = new ServiceFactory<ITransactionHandler>();

            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(portfolioQuery, stockQuery))
                .Register<Disposal>(() => new DisposalHandler(portfolioQuery, stockQuery))
                .Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(portfolioQuery, stockQuery))
                .Register<IncomeReceived>(() => new IncomeReceivedHandler(portfolioQuery, stockQuery))
                .Register<OpeningBalance>(() => new OpeningBalanceHandler(portfolioQuery, stockQuery))
                .Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(portfolioQuery, stockQuery))
                .Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(portfolioQuery, stockQuery))
                .Register<CashTransaction>(() => new CashTransactionHandler());
        }

        public ITransactionHandler GetHandler(Transaction transaction)
        {
            return _TransactionHandlers.GetService(transaction);
        }
    }


}
