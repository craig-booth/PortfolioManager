using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class ListStapledSecurityCommand : ICommand
    {
        public string ASXCode { get; }
        public string Name { get; }
        public DateTime ListingDate { get; }
        public AssetCategory Category { get; }
        public StapledSecurityChild[] ChildSecurities { get; }

        public class StapledSecurityChild
        {
            public readonly string ASXCode;
            public readonly string Name;
            public readonly bool Trust;

            public StapledSecurityChild(string asxCode, string name, bool trust)
            {
                ASXCode = asxCode;
                Name = name;
                Trust = trust;
            }
        }

        public ListStapledSecurityCommand(string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities)
        {
            ASXCode = asxCode;
            Name = name;
            ListingDate = listingDate;
            Category = category;
            ChildSecurities = childSecurities.ToArray();
        }
    }


}
