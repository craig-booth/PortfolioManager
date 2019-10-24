using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StapledSecurityListedEvent : Event
    {
        public string ASXCode { get; set; }
        public string Name { get; set; }
        public DateTime ListingDate { get; set; }
        public AssetCategory Category { get; set; }
        public StapledSecurityChild[] ChildSecurities { get; set; }

        public StapledSecurityListedEvent(Guid entityId, int version, string asxCode, string name, DateTime listingDate, AssetCategory category, StapledSecurityChild[] childSecurities)
            : base(entityId, version)
        {
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Category = category;
            ChildSecurities = childSecurities;
        }
    }
}
