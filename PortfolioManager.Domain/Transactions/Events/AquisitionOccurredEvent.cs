using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Transactions.Events
{
    public class AquisitionOccurredEvent : TransactionOccurredEvent
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public AquisitionOccurredEvent(Guid entityId, int version, Guid transactionId, DateTime date, Guid stock, string comment)
            : base(entityId, version, transactionId, date, stock, comment)
        {

        }

    }
}
