using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Stocks
{
    public class ClosingPrice
    {
        public DateTime Date { get; }
        public decimal Price { get; }

        public ClosingPrice(DateTime date, decimal price)
        {
            Date = date;
            Price = price;
        }
    }
}
