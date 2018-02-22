using System;
using System.Collections.Generic;
using System.Text;


namespace PortfolioManager.Domain.Stocks.Commands
{
    public class AddNonTradingDayCommand : ICommand
    {
        public DateTime Date { get; }

        public AddNonTradingDayCommand(DateTime date)
        {
            Date = date;
        }
    }
}
