using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEvent
    {
        Guid Id { get; }
    }

    public interface IEventHandler<T> where T : IEvent
    {
        void ApplyEvent(T @event);
    }
}
