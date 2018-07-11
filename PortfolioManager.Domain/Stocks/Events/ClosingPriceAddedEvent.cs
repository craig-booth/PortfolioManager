using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class ClosingPricesAddedEvent : Event
    {
        public ClosingPrice[] ClosingPrices { get; set; }

        public class ClosingPrice
        {
            public DateTime Date { get; set; }
            public decimal Price { get; set; }

            public ClosingPrice(DateTime date, decimal price)
            {
                Date = date;
                Price = price;
            }
        }

        public ClosingPricesAddedEvent(Guid entityId, int version, IEnumerable<ClosingPrice> closingPrices)
            : base(entityId, version)
        {
            ClosingPrices = closingPrices.ToArray();
        }
    }
}
