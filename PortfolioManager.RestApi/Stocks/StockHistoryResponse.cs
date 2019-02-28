using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Stocks
{
    public class StockHistoryResponse 
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public DateTime ListingDate { get; set; }
        public DateTime DelistedDate { get; set; }

        public class HistoricProperties
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string ASXCode { get; set; }
            public string Name { get; set; }

            public AssetCategory Category { get; set; }
        }

        public List<HistoricProperties> History { get; set; }

        public class HistoricDividendRules
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public decimal CompanyTaxRate { get; set; }
            public RoundingRule DividendRoundingRule { get; set; }
            public bool DRPActive { get; set; }
            public DRPMethod DRPMethod { get; set; }
        }

        public List<HistoricDividendRules> DividendRules { get; set; }

        public StockHistoryResponse()
        {
            History = new List<HistoricProperties>();
            DividendRules = new List<HistoricDividendRules>();
        }

    }
}
