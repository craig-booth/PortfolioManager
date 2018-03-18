using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class CapitalReturnAddedEvent : IEvent
    {
        public Guid Id { get; private set; }
        public Guid ActionId { get; private set; }
        public DateTime ActionDate { get; private set; }
        public string Description { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal Amount { get; private set; }

        public CapitalReturnAddedEvent(Guid id, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
        {
            Id = id;
            ActionId = actionId;
            ActionDate = actionDate;
            Description = description;
            PaymentDate = paymentDate;
            Amount = amount;
        }
    }
}
