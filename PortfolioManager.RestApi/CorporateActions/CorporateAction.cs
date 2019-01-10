using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public abstract class CorporateAction
    {
        public Guid Id { get; set; }
        public Guid Stock { get; set; }
        public abstract string Type { get; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }
    }     
}
