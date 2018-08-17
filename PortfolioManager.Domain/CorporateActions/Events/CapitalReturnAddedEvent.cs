using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class CapitalReturnAddedEvent : CorporateActionAddedEvent
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        public CapitalReturnAddedEvent(Guid entityId, int version, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
            : base(entityId, version, actionId, actionDate, description)
        {
            PaymentDate = paymentDate;
            Amount = amount;
        }
    }
}
