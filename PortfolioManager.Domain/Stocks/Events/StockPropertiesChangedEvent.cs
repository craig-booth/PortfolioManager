using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockPropertiesChangedEvent : Event
    {
        public DateTime ChangeDate { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }
        public AssetCategory Category { get; set; }

        public StockPropertiesChangedEvent(Guid entityId, int version, DateTime changeDate, string asxCode, string name, AssetCategory category)
            : base(entityId, version)
        {
            ChangeDate = changeDate;
            ASXCode = asxCode;
            Name = name;
            Category = category;
        }
    }
}
