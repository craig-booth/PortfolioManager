using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class ChangeStockCommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime ChangeDate { get; }

        public string NewASXCode { get; }
        public string Name { get; }
        public AssetCategory Category { get; }

        public ChangeStockCommand(string asxCode, DateTime changeDate, string newASXCode, string name, AssetCategory category)
        {
            ASXCode = asxCode;
            ChangeDate = changeDate;
            NewASXCode = newASXCode;
            Name = name;
            Category = category;
        }
    }
}
