using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class CapitalReturnAddedEvent : Event
    {
        public Guid ActionId { get; set; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public CapitalReturnAddedEvent(Guid entityId, int version, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
            : base(entityId, version)
        {
            ActionId = actionId;
            ActionDate = actionDate;
            Description = description;
            PaymentDate = paymentDate;
            Amount = amount;
        }
    }
}
