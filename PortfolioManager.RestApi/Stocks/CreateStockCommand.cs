using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Stocks
{
    public class CreateStockCommand
    {
        public Guid Id { get; set; }
        public DateTime ListingDate { get; set; }
        public string AsxCode { get; set; }
        public string Name { get; set; }
        public bool Trust { get; set; }
        public AssetCategory Category { get; set; }
        public List<StapledSecurityChild> ChildSecurities { get; }

        public class StapledSecurityChild
        {
            public string ASXCode { get; set; }
            public string Name { get; set; }
            public bool Trust { get; set; }

            public StapledSecurityChild(string asxCode, string name, bool trust)
            {
                ASXCode = asxCode;
                Name = name;
                Trust = trust;
            }
        } 

        public CreateStockCommand()
        {
            ChildSecurities = new List<StapledSecurityChild>();
        }
    }

}
