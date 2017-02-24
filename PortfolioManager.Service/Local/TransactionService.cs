﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Local
{
    class TransactionService : ITransactionService
    {
        private readonly Obsolete.TransactionService _TransactionService;

        public TransactionService(Obsolete.TransactionService transactionService)
        {
            _TransactionService = transactionService;
        }

        public Task<AddTransactionsResponce> AddTransaction(Transaction transaction)
        {
            var responce = new AddTransactionsResponce();

            _TransactionService.ProcessTransaction(transaction);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<AddTransactionsResponce>(responce);
        }

        public Task<AddTransactionsResponce> AddTransactions(IEnumerable<Transaction> transactions)
        {
            var responce = new AddTransactionsResponce();

            _TransactionService.ProcessTransactions(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<AddTransactionsResponce>(responce);
        }

        public Task<DeleteTransactionsResponce> DeleteTransaction(Transaction transaction)
        {
            var responce = new DeleteTransactionsResponce();

            _TransactionService.DeleteTransaction(transaction);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<DeleteTransactionsResponce>(responce);
        }

        public Task<UpdateTransactionsResponce> UpdateTransaction(Transaction transaction)
        {
            var responce = new UpdateTransactionsResponce();

            _TransactionService.UpdateTransaction(transaction);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<UpdateTransactionsResponce>(responce);
        }

        public Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            var responce = new GetTransactionsResponce();

            var transactions = _TransactionService.GetTransactions(fromDate, toDate);
            responce.Transactions.AddRange(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

        public Task<GetTransactionsResponce> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var responce = new GetTransactionsResponce();

            var transactions = _TransactionService.GetTransactions(asxCode, fromDate, toDate);
            responce.Transactions.AddRange(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

    }
}