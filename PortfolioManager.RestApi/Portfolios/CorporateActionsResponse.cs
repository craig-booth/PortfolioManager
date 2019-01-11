using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class CorporateActionsResponse
    {
        public List<CorporateActionItem> CorporateActions { get; } = new List<CorporateActionItem>();

        public class CorporateActionItem
        {
            public Guid Id { get; set; }
            public DateTime ActionDate { get; set; }
            public Stock Stock { get; set; }
            public string Description { get; set; }
        }
    }

}
