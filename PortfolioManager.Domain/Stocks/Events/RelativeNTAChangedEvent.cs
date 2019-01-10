using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{

    public class RelativeNTAChangedEvent : Event
    {
        public DateTime Date { get; set; }
        public decimal[] Percentages { get; set; }

        public RelativeNTAChangedEvent(Guid entityId, int version, DateTime date, IEnumerable<decimal> percentages)
            : base(entityId, version)
        {
            Date = date;
            Percentages = percentages.ToArray();
        }
    }
}
