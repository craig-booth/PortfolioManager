using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{ 
    public class ChangeDividendReinvestmentPlanEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public DateTime ChangeDate { get; }
        public bool DRPActive { get; }
        public RoundingRule DividendRoundingRule { get; }
        public DRPMethod DRPMethod { get; }

        public ChangeDividendReinvestmentPlanEvent(Guid id, int version, DateTime changeDate, bool drpActive, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
        {
            Id = id;
            Version = version;
            ChangeDate = changeDate;
            DRPActive = drpActive;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }
    }
}
