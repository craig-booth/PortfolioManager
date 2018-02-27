using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockListedEvent : IEvent
    {
        public Guid Id { get; }
        public string ASXCode { get; }
        public string Name { get; }
        public DateTime ListingDate { get; }
        public StockType Type { get; }
        public AssetCategory Category { get; }

        public StockListedEvent(Guid id, string asxCode, string name, DateTime listingDate, StockType type, AssetCategory category)
        {
            Id = id;
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Type = type;
            Category = category;
        }
    }
}
