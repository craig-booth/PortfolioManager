using System;
using System.Collections.Generic;
using System.Text;
using Booth.Common;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{ 
    public class ChangeDividendRulesEvent : Event
    {
        public DateTime ChangeDate { get; set; }
        public decimal CompanyTaxRate { get; set; }
        public RoundingRule DividendRoundingRule { get; set; }
        public bool DRPActive { get; set; }
        public DRPMethod DRPMethod { get; set; }

        public ChangeDividendRulesEvent(Guid entityId, int version, DateTime changeDate, decimal companyTaxRate, RoundingRule dividendRoundingRule, bool drpActive, DRPMethod drpMethod)
            :base(entityId, version)
        {
            ChangeDate = changeDate;
            CompanyTaxRate = companyTaxRate;
            DividendRoundingRule = dividendRoundingRule;
            DRPActive = drpActive;
            DRPMethod = drpMethod;
        }
    }
}
