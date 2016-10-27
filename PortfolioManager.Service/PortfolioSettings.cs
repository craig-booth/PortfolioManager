using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service
{
    public class StockSetting
    {
        public string ASXCode { get; private set; }
        public bool DRPActive { get; set; }
        public decimal DRPBalance { get; set; }

        public StockSetting(string asxCode)
        {
            ASXCode = asxCode;
            DRPActive = false;
            DRPBalance = 0.00m;
        }
    }

    public class PortfolioSettings
    {
        public Dictionary<string, StockSetting> StockSettings { get; private set; }

        public PortfolioSettings()
        {
            StockSettings = new Dictionary<string, StockSetting>();
        }
    }
}
