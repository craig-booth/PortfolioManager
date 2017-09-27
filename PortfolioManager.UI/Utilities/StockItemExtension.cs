
using PortfolioManager.Service.Interface;

namespace PortfolioManager.UI.Utilities
{
    public static class StockItemExtension
    {
        public static string FormattedCompanyName(this StockItem stock)
        {
            if (stock.ASXCode != "")
                return string.Format("{0} ({1})", stock.Name, stock.ASXCode);
            else
                return stock.Name;
        }

        public static string FormattedASXCode(this StockItem stock)
        {
            if (stock.Name != "")
                return string.Format("{0} ({1})", stock.ASXCode, stock.Name);
            else
                return stock.ASXCode;
        }
    }
}
