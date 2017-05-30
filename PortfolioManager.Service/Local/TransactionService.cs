﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service.Local
{
    class TransactionService : ITransactionService
    {
        private readonly ServiceFactory<ITransactionHandler> _TransactionHandlers;
        private readonly IPortfolioDatabase _PortfolioDatabase;

        public TransactionService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;

            _TransactionHandlers = new ServiceFactory<Transactions.ITransactionHandler>();
            _TransactionHandlers.Register<Aquisition>(() => new AquisitionHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
            _TransactionHandlers.Register<Disposal>(() => new DisposalHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
            _TransactionHandlers.Register<CostBaseAdjustment>(() => new CostBaseAdjustmentHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
            _TransactionHandlers.Register<IncomeReceived>(() => new IncomeReceivedHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
            _TransactionHandlers.Register<OpeningBalance>(() => new OpeningBalanceHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
            _TransactionHandlers.Register<ReturnOfCapital>(() => new ReturnOfCapitalHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
            _TransactionHandlers.Register<UnitCountAdjustment>(() => new UnitCountAdjustmentHandler(_PortfolioDatabase.PortfolioQuery, stockDatabase.StockQuery, stockDatabase));
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

        public Task<AddTransactionsResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new AddTransactionsResponce();

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

            return Task.FromResult<AddTransactionsResponce>(responce); 
        }

        public Task<AddTransactionsResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            var responce = new AddTransactionsResponce();

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

            return Task.FromResult<AddTransactionsResponce>(responce); 
        }

        public Task<DeleteTransactionsResponce> DeleteTransaction(TransactionItem transactionItem)
        {
            var responce = new DeleteTransactionsResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Delete(transaction);
                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<DeleteTransactionsResponce>(responce); 
        }

        public Task<UpdateTransactionsResponce> UpdateTransaction(TransactionItem transactionItem)
        {
            var responce = new UpdateTransactionsResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<UpdateTransactionsResponce>(responce); 
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

            var stock = _StockService.Get(stockId, fromDate);
            var asxCode = stock.ASXCode;

            var transactions = _PortfolioDatabase.PortfolioQuery.GetTransactions(asxCode, fromDate, toDate);
            responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

    }
}
