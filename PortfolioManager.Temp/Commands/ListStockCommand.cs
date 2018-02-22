using System;
using System.Collections.Generic;
using System.Text;

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
        public RoundingRule DividendRoundingRule { get; }
        public DRPMethod DRPMethod { get; }

        public ListStockCommand(string asxCode, string name, DateTime listingDate, StockType type, AssetCategory category, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
        {
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Type = type;
            Category = category;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }
    }
}
