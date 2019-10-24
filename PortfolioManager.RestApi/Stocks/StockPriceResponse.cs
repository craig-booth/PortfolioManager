using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioManager.RestApi.Stocks
{
    public class StockPriceResponse
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public List<ClosingPrice> ClosingPrices { get; set; }

        public StockPriceResponse()
        {
            ClosingPrices = new List<ClosingPrice>();
        }
        
    }
}
