using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class NonTradingDayAddedEvent : Event
    {
        public DateTime Date { get; set; }

        public NonTradingDayAddedEvent(Guid entityId, int version, DateTime date)
            : base(entityId, version)
        {
            Date = date;
        }
    }
}
