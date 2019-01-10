using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioManager.RestApi.Stocks
{
    public class DelistStockCommand
    {
        public Guid Id { get; set; }
        public DateTime DelistingDate { get; set; }
    }
}
