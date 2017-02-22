﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Obsolete
{
    class StockSetting
    {
        public string ASXCode { get; private set; }
        public bool DRPActive { get; set; }

        public StockSetting(string asxCode)
        {
            ASXCode = asxCode;
            DRPActive = true;
        }
    }

    class PortfolioSettingsService
    {
        private Dictionary<string, StockSetting> _StockSettings;

        public PortfolioSettingsService()
        {
            _StockSettings = new Dictionary<string, StockSetting>();
        }

        public StockSetting Get(string asxCode)
        {
            StockSetting stockSetting;

            if (! _StockSettings.TryGetValue(asxCode, out stockSetting))
            {
                stockSetting = new StockSetting(asxCode);
                _StockSettings.Add(asxCode, stockSetting);
            }

            return stockSetting;
        }
    }
}
