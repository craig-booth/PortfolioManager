using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.EventStore
{
    public abstract class Event
    {
        public Guid EntityId { get; set; }
        public int Version { get; set; }

        public Event(Guid entityId, int version)
        {
            EntityId = entityId;
            Version = version;
        }
    }

}
