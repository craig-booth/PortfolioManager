using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PortfolioManager.UI.Utilities
{
    class ApplicationSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue ("portfolio.db")]
        public string PortfolioDatabase
        { 
            get
            {
                return (string)this["PortfolioDatabase"];
            }
            set
            {
                this["PortfolioDatabase"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("Stocks.db")]
        public string StockDatabase
        {
            get
            {
                return (string)this["StockDatabase"];
            }
            set
            {
                this["StockDatabase"] = value;
            }
        }

        public void OnDatabaseChanged()
        {
            var handler = DatabaseChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler DatabaseChanged;
    }
}
