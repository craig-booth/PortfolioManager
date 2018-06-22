using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StapledSecurityListedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public string ASXCode { get; }
        public string Name { get; }
        public DateTime ListingDate { get; }
        public AssetCategory Category { get; }
        public StapledSecurityChild[] ChildSecurities { get; }

        public StapledSecurityListedEvent(Guid id, int version, string asxCode, string name, DateTime listingDate, AssetCategory category, StapledSecurityChild[] childSecurities)
        {
            Id = id;
            Version = version;
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Category = category;
            ChildSecurities = childSecurities;
        }
    }
}
