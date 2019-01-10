using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class Aquisition : Transaction
    {
        public override string Type
        {
            get { return "aquisition"; }
        }
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }
    }
}
