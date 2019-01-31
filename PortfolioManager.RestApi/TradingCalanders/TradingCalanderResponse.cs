using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.TradingCalanders
{
    public class TradingCalanderResponse
    {
        public int Year { get; set; }
        public List<NonTradingDay> NonTradingDays { get; }

        public class NonTradingDay
        {
            public DateTime Date { get; set; }
            public string Desciption { get; set; }

            public NonTradingDay(DateTime date, string description)
            {
                Date = date;
                Desciption = description;
            }
        }
    }
}
