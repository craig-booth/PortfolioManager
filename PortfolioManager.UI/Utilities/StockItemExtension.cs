using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.UI.Utilities
{
    public static class StockItemExtension
    {
        public static string FormattedCompanyName(this StockItem stock)
        {
            return string.Format("{0} ({1})", stock.Name, stock.ASXCode);
        }

        public static string FormattedASXCode(this StockItem stock)
        {
            return string.Format("{0} ({1})", stock.ASXCode, stock.Name);
        }
    }
}
