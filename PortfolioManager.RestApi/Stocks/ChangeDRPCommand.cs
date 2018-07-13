using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Stocks
{
    public class ChangeDividendReinvestmentPlanCommand
    {
        public Guid Id { get; set; }
        public DateTime ChangeDate { get; set; }
        public bool DRPActive { get; set; }
        public RoundingRule DividendRoundingRule { get; set; }
        public DRPMethod DRPMethod { get; set; }
    }
}
