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
        public readonly StockType Type;
        public readonly Guid[] ChildSecurities;

        public StockListedEvent(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, StockType type, Guid[] childSecurities)
        {
            Id = id;
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Type = type;
            Category = category;
            ChildSecurities = childSecurities;
        }
    }
}
