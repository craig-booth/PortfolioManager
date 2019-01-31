using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.TradingCalanders.Events
{
    public class NonTradingDaysSetEvent : Event
    {
        public int Year { get; set; }
        public List<NonTradingDay> NonTradingDays { get; set; }

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

        public NonTradingDaysSetEvent(Guid entityId, int version, int year, IEnumerable<TradingCalanders.NonTradingDay> nonTradingDays)
            : base(entityId, version)
        {
            Year = year;

            NonTradingDays = new List<NonTradingDay>();
            NonTradingDays.AddRange(nonTradingDays.Select(x => new NonTradingDay(x.Date, x.Desciption)));
        }
    }
}
