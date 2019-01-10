using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public interface ITransaction
    {
        string Type { get; }
    }

    public abstract class Transaction
    {
        public Guid Id { get; set; }
        public Guid Stock { get; set; }
        public abstract string Type { get; }
        public DateTime RecordDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
    }
}
