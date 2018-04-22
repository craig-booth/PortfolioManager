using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain
{
    public interface IEvent
    {
        Guid Id { get; }
        int Version { get; }
    }

}
