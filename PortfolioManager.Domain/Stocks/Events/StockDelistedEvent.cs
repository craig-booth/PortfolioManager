using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockDelistedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public DateTime DelistedDate { get; }

        public StockDelistedEvent(Guid id, int version, DateTime delistedDate)
        {
            Id = id;
            Version = version;
            DelistedDate = delistedDate;
        }
    }
}
