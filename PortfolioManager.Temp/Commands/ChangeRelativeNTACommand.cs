using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class ChangeRelativeNTACommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime ChangeDate { get; }

        public decimal[] Percentages { get; }

        public ChangeRelativeNTACommand(string asxCode, DateTime changeDate, IEnumerable<decimal> percentages)
        {
            ASXCode = asxCode;
            ChangeDate = changeDate;
            Percentages = percentages.ToArray();
        }
    }
}
