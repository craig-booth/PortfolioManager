using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class AddClosingPriceCommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime Date { get; }
        public decimal ClosingPrice { get; }

        public AddClosingPriceCommand(string asxCode, DateTime date, decimal closingPrice)
        {
            ASXCode = asxCode;
            Date = date;
            ClosingPrice = closingPrice;
        }
    }
}
