using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using PortfolioManager.Domain;
using PortfolioManager.Domain.TradingCalanders;
using PortfolioManager.DataServices;

namespace PortfolioManager.Web.DataImporters
{
    public class TradingDayImporter
    {
        private readonly ITradingCalander _TradingCalander;
        private readonly ITradingDayService _DataService;
        private readonly ILogger _Logger;

        public TradingDayImporter(IRepository<TradingCalander> tradingCalanderRepository, ITradingDayService dataService, ILogger<TradingDayImporter> logger)
        {
            _TradingCalander = tradingCalanderRepository.Get(TradingCalanderIds.ASX);
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
                _TradingCalander.SetNonTradingDays(year, nonTradingDays);
            }           
        }
    }
}
