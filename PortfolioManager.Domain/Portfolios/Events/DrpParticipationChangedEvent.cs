using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class DrpParticipationChangedEvent : Event
    {
        public Guid Holding { get; set; }
        public DateTime Date { get; set; }
        public bool ParticipateInDrp { get; set; }

        public DrpParticipationChangedEvent(Guid entityId, int version, Guid holding, DateTime date, bool participateInDrp)
            : base(entityId, version)
        {
            Holding = holding;
            Date = date;
            ParticipateInDrp = participateInDrp;
        }

    }
}
