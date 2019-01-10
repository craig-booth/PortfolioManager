using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class PortfolioPropertiesResponse
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Stock> StocksHeld { get; } = new List<Stock>();
    }
}
