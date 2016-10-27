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
        public decimal DRPCashBalance { get; set; }

        public StockSetting(string asxCode)
        {
            ASXCode = asxCode;
            DRPActive = true;
            DRPCashBalance = 0.00m;
        }
    }

    public class StockSettingService
    {
        public Dictionary<string, StockSetting> _StockSettings;

        public StockSettingService()
        {
            _StockSettings = new Dictionary<string, StockSetting>();
        }

        public StockSetting Get(string asxCode)
        {
            StockSetting setting;

            if (!_StockSettings.TryGetValue(asxCode, out setting))
            {
                setting = new StockSetting(asxCode);
                _StockSettings.Add(asxCode, setting);
            }

            return setting;
        }
    }
}
