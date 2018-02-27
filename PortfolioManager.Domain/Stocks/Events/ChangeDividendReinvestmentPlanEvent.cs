using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Events
{ 
    public class ChangeDividendReinvestmentPlanEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime ChangeDate { get; }
        public bool DRPActive { get; }
        public RoundingRule DividendRoundingRule { get; }
        public DRPMethod DRPMethod { get; }

        public ChangeDividendReinvestmentPlanEvent(Guid id, DateTime changeDate, bool drpActive, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
        {
            Id = id;
            ChangeDate = changeDate;
            DRPActive = drpActive;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }
    }
}
