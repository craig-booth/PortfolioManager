using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Transactions
{
    public class CostBaseAdjustment : Transaction
    {
        public override string Type
        {
            get { return "costbaseadjustment"; }
        }
        public decimal Percentage { get; set; }
    }
}
