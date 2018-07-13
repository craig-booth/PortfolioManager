using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public abstract class AddCorporateActionCommand
    {
        public Guid Id { get; set; }
        public Guid Stock { get; set; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }
    }
}
