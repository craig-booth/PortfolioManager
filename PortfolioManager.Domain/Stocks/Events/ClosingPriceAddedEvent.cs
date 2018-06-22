using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class ClosingPriceAddedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public DateTime Date { get; }
        public decimal ClosingPrice { get; }

        public ClosingPriceAddedEvent(Guid id, int version,  DateTime date, decimal closingPrice)
        {
            Id = id;
            Version = version;
            Date = date;
            ClosingPrice = closingPrice;
        }
    }
}
