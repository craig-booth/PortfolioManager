using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Transactions.Events
{
    public class DisposalOccurredEvent : TransactionOccurredEvent
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public bool CreateCashTransaction { get; set; }

        public DisposalOccurredEvent(Guid entityId, int version, Guid transactionId, DateTime date, Guid stock, string comment)
            : base(entityId, version, transactionId, date, stock, comment)
        {

        }

    }
}
