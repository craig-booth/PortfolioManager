using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Responses
{
    public class StockPriceResponse
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public class ClosingPrice
        {
            public string Date { get; }
            public decimal Price { get; }

            public ClosingPrice(DateTime date, decimal price)
            {
                Date = date.ToString("yyyy-MM-dd");
                Price = price;
            }
        }

        public List<ClosingPrice> ClosingPrices { get; set; }

        public StockPriceResponse()
        {
            ClosingPrices = new List<ClosingPrice>();
        }
        
    }
}
