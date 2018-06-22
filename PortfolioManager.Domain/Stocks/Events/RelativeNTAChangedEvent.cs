using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{

    public class RelativeNTAChangedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public DateTime Date { get; }
        public decimal[] Percentages { get; }

        public RelativeNTAChangedEvent(Guid id, int version, DateTime date, IEnumerable<decimal> percentages)
        {
            Id = id;
            Version = version;
            Date = date;
            Percentages = percentages.ToArray();
        }
    }
}
