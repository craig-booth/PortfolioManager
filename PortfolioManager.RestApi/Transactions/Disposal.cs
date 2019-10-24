using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class Disposal : Transaction
    {
        public override string Type => TransactionType.Disposal.ToRestName();
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public string CGTMethod { get; set; }
        public bool CreateCashTransaction { get; set; }
    }
}
