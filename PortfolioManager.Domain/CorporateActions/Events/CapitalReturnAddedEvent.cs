using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class CapitalReturnAddedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public Guid ActionId { get;  }
        public DateTime ActionDate { get; }
        public string Description { get;  }
        public DateTime PaymentDate { get; }
        public decimal Amount { get; }

        public CapitalReturnAddedEvent(Guid id, int version, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
        {
            Id = id;
            Version = version;
            ActionId = actionId;
            ActionDate = actionDate;
            Description = description;
            PaymentDate = paymentDate;
            Amount = amount;
        }
    }
}
