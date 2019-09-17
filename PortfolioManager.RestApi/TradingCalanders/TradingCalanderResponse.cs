using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.TradingCalanders
{
    public class TradingCalanderResponse
    {
        public int Year { get; set; }
        public List<NonTradingDay> NonTradingDays { get; }
    }
}
