using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class CashTransaction : Transaction
    {
        public override string Type => TransactionType.CashTransaction.ToRestName();
        public string CashTransactionType { get; set; }
        public decimal Amount { get; set; }
    }
}
