using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;

namespace PortfolioManager.Test
{
    public class PortfolioManagerSettings
    {
        public string PortfolioDatabaseFile { get; set; }
        public string StockDatabaseFile { get; set; }

        public static PortfolioManagerSettings Load()
        {

            var portfolioManagerKey = Registry.CurrentUser.CreateSubKey("Portfolio Manager");
            var settingsKey = portfolioManagerKey.OpenSubKey("Settings");
            if (settingsKey != null)
            {
                PortfolioManagerSettings settings = new PortfolioManagerSettings();
                settings.PortfolioDatabaseFile = settingsKey.GetValue("Portfolio Database", "").ToString();
                settings.StockDatabaseFile = settingsKey.GetValue("Stock Database", "").ToString();
                return settings;
            }
            else
                return null;
        }

        public void Save()
        {
            var portfolioManagerKey = Registry.CurrentUser.CreateSubKey("Portfolio Manager");
            var settingsKey = portfolioManagerKey.CreateSubKey("Settings");

            settingsKey.SetValue("Portfolio Database", PortfolioDatabaseFile);
            settingsKey.SetValue("Stock Database", StockDatabaseFile);
        }
    }
}
