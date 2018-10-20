using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class TransactionsResponse
    {
        public List<TransactionItem> Transactions = new List<TransactionItem>();

        public class TransactionItem
        {
            public Stock Stock { get; set; }
            public string Type { get; set; }
            public DateTime TransactionDate { get; set; }
            public DateTime RecordDate { get; set; }
            public string Description { get; set; }
            public string Comment { get; set; }
        }
    }
}
