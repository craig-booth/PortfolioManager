using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

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
    }

}
