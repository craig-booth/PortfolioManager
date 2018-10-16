using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Portfolios
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }
        public AssetCategory Category { get; set; }
    }
}
