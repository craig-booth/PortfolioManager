using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Transactions
{
    public interface ITransactionCollection
    {
        Transaction Get(Guid id);
        void Add(Transaction transaction);

        IEnumerable<Transaction> All();
        IEnumerable<Transaction> All(DateRange dateRange);
    }

    public class TransactionCollection : ITransactionCollection
    {

        private Dictionary<Guid, Transaction> _Transactions = new Dictionary<Guid, Transaction>();

        public IEnumerable<Transaction> All()
        {
            return _Transactions.Values;
        }

        public IEnumerable<Transaction> All(DateRange dateRange)
        {
            return _Transactions.Values.Where(x => dateRange.Contains(x.TransactionDate));
        }

        public Transaction Get(Guid id)
        {
            if (_Transactions.ContainsKey(id))
                return _Transactions[id];
            else
                return null;
        }

        public void Add(Transaction transaction)
        {
            _Transactions.Add(transaction.Id, transaction);
        }
    }
}
