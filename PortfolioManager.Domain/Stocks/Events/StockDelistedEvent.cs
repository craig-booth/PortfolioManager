using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockDelistedEvent : Event
    {
        public DateTime DelistedDate { get; set; }

        public StockDelistedEvent(Guid entityId, int version, DateTime delistedDate)
            : base(entityId, version)
        {
            DelistedDate = delistedDate;
        }
    }
}
