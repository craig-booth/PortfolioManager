using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class ChangeDividendReinvestmentPlanCommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime ChangeDate { get; }

        public bool DRPActive { get; }
        public DRPMethod Method { get; }
        public RoundingRule RoundingRule { get; }

        public ChangeDividendReinvestmentPlanCommand(string asxCode, DateTime changeDate, bool drpActive, DRPMethod method, RoundingRule roundingRule)
        {
            ASXCode = asxCode;
            ChangeDate = changeDate;
            DRPActive = drpActive;
            Method = method;
            RoundingRule = roundingRule;
        }
    }
}
