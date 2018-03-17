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
        public bool Trust { get; }
        public AssetCategory Category { get; }

        public ListStockCommand(string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category)
        {
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Trust = trust;
            Category = category;
        }
    }
}
