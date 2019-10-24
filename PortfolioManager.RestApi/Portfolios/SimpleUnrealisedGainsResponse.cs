using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class SimpleUnrealisedGainsResponse
    {
        public List<SimpleUnrealisedGainsItem> UnrealisedGains { get; } = new List<SimpleUnrealisedGainsItem>();
    }

    public class SimpleUnrealisedGainsItem
    {
        public Stock Stock { get; set; }

        public DateTime AquisitionDate { get; set; }
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public decimal MarketValue { get; set; }
        public decimal CapitalGain { get; set; }
        public decimal DiscoutedGain { get; set; }
        public CGTMethod DiscountMethod { get; set; }
    }
}
