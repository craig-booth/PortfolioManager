using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Stocks
{
    public class StockPrice
    {
        public string ASXCode { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }

        public StockPrice()
        {

        }

        public StockPrice(string asxCode, DateTime time, decimal price)
        {
            ASXCode = asxCode;
            Time = time;
            Price = price;
        }
    }
}
