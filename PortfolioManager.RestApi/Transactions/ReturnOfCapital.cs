using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class ReturnOfCapital : Transaction
    {
        public override string Type
        {
            get { return "returnofcapital"; }
        }
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }
    }
}
