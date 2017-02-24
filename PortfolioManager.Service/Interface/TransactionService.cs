using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Service.Interface
{
    public interface ITransactionService : IPortfolioManagerService
    {
        Task<AddTransactionsResponce> AddTransaction(Transaction transaction);
        Task<AddTransactionsResponce> AddTransactions(IEnumerable<Transaction> transactions);
        Task<UpdateTransactionsResponce> UpdateTransaction(Transaction transaction);
        Task<DeleteTransactionsResponce> DeleteTransaction(Transaction transaction);

        Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate);
        Task<GetTransactionsResponce> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate);
    }

    public class AddTransactionsResponce : ServiceResponce
    {
        public AddTransactionsResponce()
            : base()
        {
        }
    }

    public class UpdateTransactionsResponce : ServiceResponce
    {
        public UpdateTransactionsResponce()
            : base()
        {
        }
    }

    public class DeleteTransactionsResponce : ServiceResponce
    {
        public DeleteTransactionsResponce()
            : base()
        {
        }
    }

    public class GetTransactionsResponce : ServiceResponce
    {
        public List<Transaction> Transactions;

        public GetTransactionsResponce()
            : base()
        {
            Transactions = new List<Transaction>();
        }
    }
}
