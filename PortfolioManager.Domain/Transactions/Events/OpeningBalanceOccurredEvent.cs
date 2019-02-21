using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Transactions.Events
{
    public class OpeningBalanceOccurredEvent : TransactionOccurredEvent
    {
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public OpeningBalanceOccurredEvent(Guid entityId, int version, Guid transactionId, DateTime date, Guid stock, string comment)
            : base(entityId, version, transactionId, date, stock, comment)
        {

        }

    }
}
