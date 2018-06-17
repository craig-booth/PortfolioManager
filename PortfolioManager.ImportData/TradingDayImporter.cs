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
        private readonly StockExchange _StockExchange;
        private readonly ITradingDayService _DataService;
        private readonly ILogger _Logger;

        public TradingDayImporter(StockExchange stockExchange, ITradingDayService dataService, ILogger<TradingDayImporter> logger)
        {
            _StockExchange = stockExchange;
            _DataService = dataService;
            _Logger = logger;
        }

        public async Task Import(CancellationToken cancellationToken)
        {
            var nonTradingDays = await _DataService.NonTradingDays(DateTime.Today.Year, cancellationToken);

            foreach (var nonTradingDay in nonTradingDays)
            {
                if (!_StockExchange.TradingCalander.IsTradingDay(nonTradingDay))
                {
                    _Logger?.LogInformation("Adding non-trading day {0:d}", nonTradingDay);
                    _StockExchange.TradingCalander.AddNonTradingDay(nonTradingDay);
                }
            }
            
        }
    }
}
