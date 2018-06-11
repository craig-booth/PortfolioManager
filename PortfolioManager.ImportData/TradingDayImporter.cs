using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class TradingDayImporter
    {
        private readonly ITradingDayService _DataService;
        private readonly TradingCalander _TradingCalendar;

        public TradingDayImporter(TradingCalander tradingCalander, ITradingDayService dataService)
        {
            _TradingCalendar = tradingCalander;
            _DataService = dataService;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var nonTradingDays = await _DataService.NonTradingDays(DateTime.Today.Year, cancellationToken);

            foreach (var nonTradingDay in nonTradingDays)
            {
                if (!_TradingCalendar.IsTradingDay(nonTradingDay))
                    _TradingCalendar.AddNonTradingDay(nonTradingDay);
            }
            
        }
    }
}
