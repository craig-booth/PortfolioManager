using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common; 

namespace PortfolioManager.RestApi.CorporateActions
{
    public class CorporateActionResponse
    {
        public Guid Id { get; set; }
        public Guid Stock { get; set; }
        public CorporateActionType Type { get; set; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }       
    }
}
