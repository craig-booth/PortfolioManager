using System;
using System.Collections.Generic;
using System.Text;


namespace PortfolioManager.Domain.Transactions.Events
{
    public class ReturnOfCapitalOccurredEvent : TransactionOccurredEvent
    {
        public DateTime RecordDate { get; set; }
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; }

        public ReturnOfCapitalOccurredEvent(Guid entityId, int version, Guid transactionId, DateTime date, Guid stock, string comment)
            : base(entityId, version, transactionId, date, stock, comment)
        {

        }

    }
}
