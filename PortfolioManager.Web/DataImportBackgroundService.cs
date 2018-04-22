using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Web
{
    public class DataImportBackgroundService : BackgroundService
    {

        private readonly HistoricalPriceImporter _HistoricalPriceImporter;
        private readonly TimeSpan _RunTime;
        public DataImportBackgroundService(StockExchange stockExchange, TimeSpan runTime)
        {
            var dataService = new ASXDataService();

            _HistoricalPriceImporter = new HistoricalPriceImporter(stockExchange, dataService);

            _RunTime = runTime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _HistoricalPriceImporter.Import(stoppingToken);

                var nextRun = DateTime.Today.Add(_RunTime);
                if (nextRun <= DateTime.Now)
                    nextRun.AddDays(1);

                var delay = nextRun.Subtract(DateTime.Now);

                await Task.Delay(delay, stoppingToken);
             }

        }
    }
}
