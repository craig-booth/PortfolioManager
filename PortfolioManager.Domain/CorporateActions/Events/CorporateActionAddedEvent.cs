using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class CorporateActionAddedEvent : Event
    {
        public Guid ActionId { get; set; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }

        public CorporateActionAddedEvent(Guid entityId, int version, Guid actionId, DateTime actionDate, string description)
            : base(entityId, version)
        {
            ActionId = actionId;
            ActionDate = actionDate;
            Description = description;
        }
    }
}
