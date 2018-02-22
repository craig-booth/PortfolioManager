using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class NonTradingDayAddedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime Date { get; }

        public NonTradingDayAddedEvent(DateTime date)
        {
            Id = Guid.Empty;
            Date = date;
        }
    }
}
