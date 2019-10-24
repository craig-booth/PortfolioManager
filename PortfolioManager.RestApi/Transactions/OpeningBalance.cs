using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class OpeningBalance : Transaction
    {
        public override string Type => TransactionType.OpeningBalance.ToRestName();
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }
    }
}
