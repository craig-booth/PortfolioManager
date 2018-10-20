using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Portfolios
{
    public class CGTLiabilityResponse
    {
        public decimal CurrentYearCapitalGainsOther { get; set; }
        public decimal CurrentYearCapitalGainsDiscounted { get; set; }
        public decimal CurrentYearCapitalGainsTotal { get; set; }
        public decimal CurrentYearCapitalLossesOther { get; set; }
        public decimal CurrentYearCapitalLossesDiscounted { get; set; }
        public decimal CurrentYearCapitalLossesTotal { get; set; }
        public decimal GrossCapitalGainOther { get; set; }
        public decimal GrossCapitalGainDiscounted { get; set; }
        public decimal GrossCapitalGainTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal NetCapitalGainOther { get; set; }
        public decimal NetCapitalGainDiscounted { get; set; }
        public decimal NetCapitalGainTotal { get; set; }

        public List<CGTLiabilityEvent> Events { get; } = new List<CGTLiabilityEvent>();

        public class CGTLiabilityEvent
        {
            public Stock Stock { get; set; }
            public DateTime EventDate { get; set; }
            public decimal CostBase { get; set; }
            public decimal AmountReceived { get; set; }
            public decimal CapitalGain { get; set; }
            public CGTMethod Method { get; set; }
        }
    }

}
