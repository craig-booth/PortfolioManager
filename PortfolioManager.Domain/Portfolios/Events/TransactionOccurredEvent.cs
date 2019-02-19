using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public abstract class TransactionnOccurredEvent : Event
    {
        public DateTime Date { get; set; }
        public Guid Stock { get; set; }
        public string Comment { get; set; }

        public TransactionnOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
            : base(entityId, version)
        {
            Date = date;
            Stock = stock;
            Comment = comment;
        }

    }
}
