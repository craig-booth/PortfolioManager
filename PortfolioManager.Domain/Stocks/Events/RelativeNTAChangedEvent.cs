using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioManager.Domain.Stocks.Events
{

    public class RelativeNTAChangedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime Date { get; }
        public decimal[] Percentages { get; }

        public RelativeNTAChangedEvent(Guid id, DateTime date, IEnumerable<decimal> percentages)
        {
            Id = id;
            Date = date;
            Percentages = percentages.ToArray();
        }
    }
}
