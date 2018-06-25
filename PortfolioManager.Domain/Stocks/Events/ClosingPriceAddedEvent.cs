using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class ClosingPriceAddedEvent : Event
    {
        public DateTime Date { get; set; }
        public decimal ClosingPrice { get; set; }

        public ClosingPriceAddedEvent(Guid entityId, int version,  DateTime date, decimal closingPrice)
            : base(entityId, version)
        {
            Date = date;
            ClosingPrice = closingPrice;
        }
    }
}
