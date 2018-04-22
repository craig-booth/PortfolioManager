using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class NonTradingDayAddedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public DateTime Date { get; }

        public NonTradingDayAddedEvent(Guid id, int version, DateTime date)
        {
            Id = id;
            Version = version;
            Date = date;
        }
    }
}
