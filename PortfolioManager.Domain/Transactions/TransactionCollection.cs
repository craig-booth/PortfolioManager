﻿using System;
using System.Collections.Generic;
using System.Linq;
using Booth.Common;

using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.Transactions
{

    public interface ITransactionHandler
    {
        void ApplyTransaction(Transaction transaction);
    }

    public interface ITransactionCollection : ITransactionList<Transaction>
    {
        IEnumerable<Transaction> ForHolding(Guid stockId);
        IEnumerable<Transaction> ForHolding(Guid stockId, DateRange dateRange);
    }

    class TransactionCollection :
        TransactionList<Transaction>,
        ITransactionCollection
    {

        public new void Add(Transaction transaction)
        {
            base.Add(transaction);
        }

        public IEnumerable<Transaction> ForHolding(Guid stockId)
        {
            return this.Where(x => (x.Stock != null) && (x.Stock.Id == stockId));
        }

        public IEnumerable<Transaction> ForHolding(Guid stockId, DateRange dateRange)
        {
            return InDateRange(dateRange).Where(x => (x.Stock != null) && (x.Stock.Id == stockId));
        }
    }
}
