using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Transactions.Events
{
    public abstract class TransactionOccurredEvent : Event
    {
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public Guid Stock { get; set; }
        public string Comment { get; set; }

        public TransactionOccurredEvent(Guid entityId, int version, Guid transactionId, DateTime date, Guid stock, string comment)
            : base(entityId, version)
        {
            TransactionId = transactionId;
            Date = date;
            Stock = stock;
            Comment = comment;
        }

    }
}
