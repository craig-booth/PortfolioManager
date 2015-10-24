using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Stocks
{
    public class StockPrice
    {
        public Guid Stock { get; private set; }
        public DateTime Date { get; private set; }
        public decimal Price { get; private set; }

        public StockPrice(Guid stock, DateTime date, decimal price)
        {
            Stock = Stock;
            Date = date;
            Price = price;
        }
    }
}
