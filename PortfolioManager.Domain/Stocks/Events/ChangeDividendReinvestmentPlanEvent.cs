using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{ 
    public class ChangeDividendReinvestmentPlanEvent : Event
    {
        public DateTime ChangeDate { get; set; }
        public bool DRPActive { get; set; }
        public RoundingRule DividendRoundingRule { get; set; }
        public DRPMethod DRPMethod { get; set; }

        public ChangeDividendReinvestmentPlanEvent(Guid entityId, int version, DateTime changeDate, bool drpActive, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
            :base(entityId, version)
        {
            ChangeDate = changeDate;
            DRPActive = drpActive;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }
    }
}
