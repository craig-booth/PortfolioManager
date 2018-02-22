using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockDelistedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime DelistedDate { get; }

        public StockDelistedEvent(Guid stockId, DateTime delistedDate)
        {
            Id = stockId;
            DelistedDate = delistedDate;
        }
    }
}
