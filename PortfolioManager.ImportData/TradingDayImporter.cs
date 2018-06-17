using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.ImportData
{
    public class TradingDayImporter
    {
        private readonly ITradingDayService _DataService;
        private readonly TradingCalander _TradingCalendar;
        private readonly ILogger _Logger;

        public TradingDayImporter(TradingCalander tradingCalander, ITradingDayService dataService, ILogger logger)
        {
            _TradingCalendar = tradingCalander;
            _DataService = dataService;
            _Logger = logger;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var nonTradingDays = await _DataService.NonTradingDays(DateTime.Today.Year, cancellationToken);

            foreach (var nonTradingDay in nonTradingDays)
            {
                if (!_TradingCalendar.IsTradingDay(nonTradingDay))
                {
                    _Logger?.LogInformation("Adding non-trading day {0:d}", nonTradingDay);
                    _TradingCalendar.AddNonTradingDay(nonTradingDay);
                }
            }
            
        }
    }
}
