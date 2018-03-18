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
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;


namespace PortfolioManager.Service.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;
        private readonly IMapper _Mapper;

        public TransactionService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;

            var config = new MapperConfiguration(cfg => 
                cfg.AddProfile(new ModelToServiceMapping(_StockExchange))
            );
            _Mapper = config.CreateMapper();
        }

        public Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                transactionItem.Id = Guid.NewGuid();
                var transaction = _Mapper.Map<Transaction>(transactionItem);

                var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, _StockExchange);

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

            return Task.FromResult<ServiceResponce>(responce); 
        }

        public Task<ServiceResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            var responce = new ServiceResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, _StockExchange);

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
                var stock = _StockExchange.Stocks.Get(stockId);

                var transactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(stock.Properties[toDate].ASXCode, fromDate, toDate);
                responce.Transactions.AddRange(_Mapper.Map<IEnumerable<TransactionItem>>(transactions));

                responce.SetStatusToSuccessfull();
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
                var transactionHandlerFactory = new TransactionHandlerFactory(portfolioUnitOfWork.PortfolioQuery, _StockExchange);

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

        public TransactionHandlerFactory(IPortfolioQuery portfolioQuery, StockExchange stockExchange)
        {
            _TransactionHandlers = new ServiceFactory<ITransactionHandler>();

            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(portfolioQuery, stockExchange))
                .Register<Disposal>(() => new DisposalHandler(portfolioQuery, stockExchange))
                .Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(portfolioQuery, stockExchange))
                .Register<IncomeReceived>(() => new IncomeReceivedHandler(portfolioQuery, stockExchange))
                .Register<OpeningBalance>(() => new OpeningBalanceHandler(portfolioQuery, stockExchange))
                .Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(portfolioQuery, stockExchange))
                .Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(portfolioQuery, stockExchange))
                .Register<CashTransaction>(() => new CashTransactionHandler());
        }

        public ITransactionHandler GetHandler(Transaction transaction)
        {
            return _TransactionHandlers.GetService(transaction);
        }
    }


}
