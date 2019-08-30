using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Users.Events
{
    public class PasswordChangedEvent : Event
    {
        public string Password { get; set; }

        public PasswordChangedEvent(Guid id, int version, string password)
            : base(id, version)
        {
            Password = password;
        }
    }
}
