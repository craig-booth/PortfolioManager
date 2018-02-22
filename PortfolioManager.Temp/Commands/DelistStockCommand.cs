using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class DelistStockCommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime Date { get; }

        public DelistStockCommand(string asxCode, DateTime date)
        {
            ASXCode = asxCode;
            Date = date;
        }
    }
}
