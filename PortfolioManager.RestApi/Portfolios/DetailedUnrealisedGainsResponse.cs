using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Portfolios
{
    public class DetailedUnrealisedGainsResponse
    {
        public List<DetailedUnrealisedGainsItem> UnrealisedGains { get; } = new List<DetailedUnrealisedGainsItem>();
    }

    public class DetailedUnrealisedGainsItem : SimpleUnrealisedGainsItem
    {
        public List<CGTEventItem> CGTEvents { get; } = new List<CGTEventItem>();

        public class CGTEventItem
        {
            public DateTime Date { get; set; }
            public string Description { get; set; }
            public int UnitChange { get; set; }
            public int Units { get; set; }
            public decimal CostBaseChange { get; set; }
            public decimal CostBase { get; set; }
        }
    } 
    



}
