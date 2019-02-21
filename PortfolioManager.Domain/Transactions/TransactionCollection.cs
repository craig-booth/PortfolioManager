using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Utils;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions.Events;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Transactions
{

    public interface ITransactionHandler
    {
        void ApplyTransaction(Transaction transaction);
    }

    public interface ITransactionCollection : ITransactionList<Transaction>
    {

    }

    class TransactionCollection :
        TransactionList<Transaction>,
        ITransactionCollection
    {

        public new void Add(Transaction transaction)
        {
            base.Add(transaction);
        }
    }
}
