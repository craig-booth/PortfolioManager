using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class AquisitionOccurredEvent : TransactionnOccurredEvent
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public AquisitionOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
        : base(entityId, version, date, stock, comment)
        {

        }

    }
}
