using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Commands
{
    public class UpdateClosingPricesCommand
    {
        public Guid Id { get; set; }
        public List<ClosingPrice> Prices { get; }

        public class ClosingPrice
        {
            public DateTime Date { get; set; }
            public decimal Price { get; set; }

            public ClosingPrice(DateTime date, decimal price)
            {
                Date = date;
                Price = price;
            }
        }

        public UpdateClosingPricesCommand()
        {
            Prices = new List<ClosingPrice>();
        }
    }
}
