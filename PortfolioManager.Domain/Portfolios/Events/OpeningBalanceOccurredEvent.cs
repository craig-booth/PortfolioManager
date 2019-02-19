using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class OpeningBalanceOccurredEvent : TransactionnOccurredEvent
    {
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public OpeningBalanceOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
            : base(entityId, version, date, stock, comment)
        {

        }

    }
}
