using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Transactions
{
    public interface ITransaction
    {
        Guid Id { get; }
        DateTime TransactionDate { get; }
    }

    public abstract class Transaction : ITransaction
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public Stock Stock { get; set; }
        public DateTime RecordDate { get; set; }
        public string Comment { get; set; }
        public abstract string Description { get; }
    }
}
