using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.TradingCalanders
{
    public class UpdateTradingCalanderCommand
    {
        public int Year { get; set; }
        public List<NonTradingDay> NonTradingDays { get; }

        public UpdateTradingCalanderCommand()
        {
            NonTradingDays = new List<NonTradingDay>();
        }
    }    
}
