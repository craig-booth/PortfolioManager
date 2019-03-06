using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class PortfolioPropertiesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<HoldingProperties> Holdings { get; } = new List<HoldingProperties>();
    }

    public class HoldingProperties
    {
        public Stock Stock { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool ParticipatingInDrp { get; set; }
    }
}
