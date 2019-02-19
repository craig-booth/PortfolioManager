using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class DisposalOccurredEvent : TransactionnOccurredEvent
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public bool CreateCashTransaction { get; set; }

        public DisposalOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
            : base(entityId, version, date, stock, comment)
        {

        }

    }
}
