using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Test
{
    public class PortfolioManagerSettings
    {
        public string PortfolioDatabaseFile { get; set; }
        public string StockDatabaseFile { get; set; }

        public static PortfolioManagerSettings Load()
        {
            return null;
            
            
     /*       PortfolioManagerSettings settings = new PortfolioManagerSettings();

            settings.PortfolioDatabaseFile = "";
            settings.StockDatabaseFile = "";

            return settings; */
        }

        public void Save()
        {

        }
    }
}
