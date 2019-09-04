using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class PortfolioCreatedEvent :Event
    {
        public string Name { get; set; }
        public Guid Owner { get; set; }

        public PortfolioCreatedEvent(Guid entityId, int version, string name, Guid owner)
            : base(entityId, version)
        {
            Name = name;
            Owner = owner;
        }

    }
}
