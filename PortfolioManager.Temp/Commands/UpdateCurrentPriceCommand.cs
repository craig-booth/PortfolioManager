using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class UpdateCurrentPriceCommand : ICommand
    {
        public string ASXCode { get; }
        public decimal CurrentPrice { get; }

        public UpdateCurrentPriceCommand(string asxCode, decimal currentPrice)
        {
            ASXCode = asxCode;
            CurrentPrice = currentPrice;
        }
    }
}
