﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;
using PortfolioManager.Domain.Portfolios.Events;

namespace PortfolioManager.Utils
{
    static class EventStoreUtils
    {
        public static void CopyEventStore(IEventStore source, IEventStore destination)
        {
            var sourceStream = source.GetEventStream("TradingCalander");
            var destinationStream = destination.GetEventStream("TradingCalander");
            CopyEventStream(sourceStream, destinationStream);

            sourceStream = source.GetEventStream("StockRepository");
            destinationStream = destination.GetEventStream("StockRepository");
            CopyEventStream(sourceStream, destinationStream);
        }

        public static void CopyEventStream(IEventStream source, IEventStream destination)
        {
            foreach (var entity in source.GetAll())
                destination.Add(entity.EntityId, entity.Type, entity.Events);
        }

        public static void CreatePortfolio(IEventStore eventStore, Guid id, string name)
        {
            var eventStream = eventStore.GetEventStream("Portfolios");

            var events = new Event[] { new PortfolioCreatedEvent(id, 0, name) };
            eventStream.Add(id, "Portfolio", events);
        }
    }

}
