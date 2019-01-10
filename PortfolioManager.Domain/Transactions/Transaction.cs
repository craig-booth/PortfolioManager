using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Transactions
{
    public abstract class Transaction : ITransaction
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Stock Stock { get; set; }
        public DateTime RecordDate { get; set; }
        public string Comment { get; set; }
        public abstract string Description { get; }
    }
}
