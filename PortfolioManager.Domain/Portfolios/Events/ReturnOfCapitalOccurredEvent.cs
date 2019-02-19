using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class ReturnOfCapitalOccurredEvent : TransactionnOccurredEvent
    {
        public DateTime RecordDate { get; set; }
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }

        public ReturnOfCapitalOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
            : base(entityId, version, date, stock, comment)
        {

        }

    }
}
