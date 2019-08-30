﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain
{
    public interface ITrackedEntity : IEntity
    {
        IEnumerable<Event> FetchEvents();
        void ApplyEvents(IEnumerable<Event> events);
    }

    public interface IEntityProperties
    {
        IDictionary<string, string> GetProperties();
    }

    public abstract class TrackedEntity : ITrackedEntity
    {
        public Guid Id { get; }
        public int Version { get; protected set; } = 0;
        private EventList _Events = new EventList();

        public TrackedEntity(Guid id)
        {
            Id = id;
        }

        protected void PublishEvent(Event @event)
        {
            _Events.Add(@event);
        }

        public IEnumerable<Event> FetchEvents()
        {
            return _Events.Fetch();
        }

        private void Apply(Event @event)
        {
            throw new NotSupportedException();
        }

        public void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;

                var me = (dynamic)this;
                me.Apply(dynamicEvent);
            }
        }
    }
}
