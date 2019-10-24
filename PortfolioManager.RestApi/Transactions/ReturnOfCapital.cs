using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class ReturnOfCapital : Transaction
    {
        public override string Type => TransactionType.ReturnOfCapital.ToRestName();
        public DateTime RecordDate { get; set; }
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }
    }
}
