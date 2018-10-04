using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Transactions
{
    public abstract class Transaction
    {
        public Guid Id { get; set; }
        public Stock Stock { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Comment { get; set; }
        public abstract string Description { get; }
    }
}
