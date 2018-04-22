﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Events
{
    public class StockPropertiesChangedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public DateTime ChangeDate { get; }
        public string ASXCode { get; }
        public string Name { get; }
        public AssetCategory Category { get; }

        public StockPropertiesChangedEvent(Guid id, int version, DateTime changeDate, string asxCode, string name, AssetCategory category)
        {
            Id = id;
            Version = version;
            ChangeDate = changeDate;
            ASXCode = asxCode;
            Name = name;
            Category = category;
        }
    }
}
