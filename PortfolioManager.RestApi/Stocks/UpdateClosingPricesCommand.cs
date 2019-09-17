using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Stocks
{
    public class UpdateClosingPricesCommand
    {
        public Guid Id { get; set; }
        public List<ClosingPrice> Prices { get; }

        public UpdateClosingPricesCommand()
        {
            Prices = new List<ClosingPrice>();
        }
    }

}
