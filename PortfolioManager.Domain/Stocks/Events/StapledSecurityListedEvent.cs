using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StapledSecurityListedEvent : IEvent
    {
        public Guid Id { get; }
        public readonly string ASXCode;
        public readonly string Name;
        public readonly DateTime ListingDate;
        public readonly AssetCategory Category;
        public readonly StapledSecurityChild[] ChildSecurities;

        public StapledSecurityListedEvent(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, StapledSecurityChild[] childSecurities)
        {
            Id = id;
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Category = category;
            ChildSecurities = childSecurities;
        }
    }
}
