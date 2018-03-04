using System;
using System.Collections.Generic;
using System.Linq;


using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class ListStockCommand: ICommand
    {
        public string ASXCode { get; }
        public string Name { get; }
        public DateTime ListingDate { get; }
        public StockType Type { get; }
        public AssetCategory Category { get; }
        public string[] ChildSecurities { get; }

        public ListStockCommand(string asxCode, string name, DateTime listingDate, StockType type, AssetCategory category, IEnumerable<string> childSecurities)
        {
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Type = type;
            Category = category;
            if (type == StockType.StapledSecurity)
                ChildSecurities = childSecurities.ToArray();
        }
    }
}
