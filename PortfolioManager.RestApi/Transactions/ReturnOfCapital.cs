using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Transactions
{
    public class ReturnOfCapital : Transaction
    {
        public override string Type => TransactionType.ReturnOfCapital.ToRestName();
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }
    }
}
