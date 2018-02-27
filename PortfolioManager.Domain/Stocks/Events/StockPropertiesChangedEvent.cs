using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockPropertiesChangedEvent : IEvent
    {
        public Guid Id { get; }
        public DateTime ChangeDate { get; }
        public string ASXCode { get; }
        public string Name { get; }
        public StockType Type { get; }
        public AssetCategory Category { get; }

        public StockPropertiesChangedEvent(Guid id, DateTime changeDate, string asxCode, string name, StockType type, AssetCategory category)
        {
            Id = id;
            ChangeDate = changeDate;
            ASXCode = asxCode;
            Name = name;
            Type = type;
            Category = category;
        }
    }
}
