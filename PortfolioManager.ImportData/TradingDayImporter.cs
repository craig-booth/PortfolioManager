using System;
using System.Collections.Generic;
using System.Linq;
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
            int year = DateTime.Today.Year;

            var nonTradingDays = await _DataService.NonTradingDays(year, cancellationToken);

            if (nonTradingDays.Any())
            {
                _Logger?.LogInformation("Adding {0} non-trading days for {1}", nonTradingDays.Count(), year);
                _StockExchange.TradingCalander.SetNonTradingDays(year, nonTradingDays);
            }           
        }
    }
}
