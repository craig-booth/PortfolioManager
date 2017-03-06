using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
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

        public Task<AddTransactionsResponce> AddTransaction(TransactionItem transactionItem)
        {
            var responce = new AddTransactionsResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);
            _TransactionService.ProcessTransaction(transaction);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<AddTransactionsResponce>(responce); 
        }

        public Task<AddTransactionsResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            var responce = new AddTransactionsResponce();

            var transactions = Mapper.Map<List<Transaction>>(transactionItems);
            _TransactionService.ProcessTransactions(transactions);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<AddTransactionsResponce>(responce); 
        }

        public Task<DeleteTransactionsResponce> DeleteTransaction(TransactionItem transactionItem)
        {
            var responce = new DeleteTransactionsResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);
            _TransactionService.DeleteTransaction(transaction);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<DeleteTransactionsResponce>(responce); 
        }

        public Task<UpdateTransactionsResponce> UpdateTransaction(TransactionItem transactionItem)
        {
            var responce = new UpdateTransactionsResponce();

            var transaction = Mapper.Map<Transaction>(transactionItem);
            _TransactionService.UpdateTransaction(transaction);

            responce.SetStatusToSuccessfull();

            return Task.FromResult<UpdateTransactionsResponce>(responce); 
        }

        public Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate)
        {       
            var responce = new GetTransactionsResponce();

            var transactions = _TransactionService.GetTransactions(fromDate, toDate);
            responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));
            
            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce); 
        }

        public Task<GetTransactionsResponce> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var responce = new GetTransactionsResponce();

            var transactions = _TransactionService.GetTransactions(asxCode, fromDate, toDate);
            responce.Transactions.AddRange(Mapper.Map<IEnumerable<TransactionItem>>(transactions));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<GetTransactionsResponce>(responce);
        }

    }
}
