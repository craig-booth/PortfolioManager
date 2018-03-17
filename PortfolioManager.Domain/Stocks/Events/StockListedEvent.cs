using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockListedEvent : IEvent
    {
        public Guid Id { get; }
        public readonly string ASXCode;
        public readonly string Name;
        public readonly DateTime ListingDate;
        public readonly AssetCategory Category;
        public readonly bool Trust;

        public StockListedEvent(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, bool trust)
        {
            Id = id;
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Trust = trust;
            Category = category;
        }
    }
}
