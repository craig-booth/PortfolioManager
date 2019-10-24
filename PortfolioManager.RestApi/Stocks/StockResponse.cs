using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booth.Common;

namespace PortfolioManager.RestApi.Stocks
{
    public class StockResponse
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public AssetCategory Category { get; set; }
        public bool Trust { get; set; }
        public bool StapledSecurity { get; set; }

        public DateTime ListingDate { get; set; }
        public DateTime DelistedDate { get; set; }

        public decimal LastPrice { get; set; }

        public decimal CompanyTaxRate { get; set; }
        public RoundingRule DividendRoundingRule { get; set; }
        public bool DRPActive { get; set; }
        public DRPMethod DRPMethod { get; set; }

        public class StapledSecurityChildResponse
        {
            public string ASXCode { get; set; }
            public string Name { get; set; }
            public bool Trust { get; set; }
        }

        public List<StapledSecurityChildResponse> ChildSecurities { get; set; }

        public StockResponse()
        {
            ChildSecurities = new List<StapledSecurityChildResponse>();
        }
    }
}
