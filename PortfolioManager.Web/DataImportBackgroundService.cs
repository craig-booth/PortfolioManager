using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

using PortfolioManager.Common.Scheduler;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Web
{
    public class DataImportBackgroundService : BackgroundService
    {
        private Scheduler _Scheduler;

        private readonly HistoricalPriceImporter _HistoricalPriceImporter;
        private readonly LivePriceImporter _LivePriceImporter;
        private readonly TradingDayImporter _TradingDayImporter;

        public DataImportBackgroundService(StockExchange stockExchange)
        {
            var dataService = new ASXDataService();
            _HistoricalPriceImporter = new HistoricalPriceImporter(stockExchange, dataService);
            _LivePriceImporter = new LivePriceImporter(stockExchange, dataService);
            _TradingDayImporter = new TradingDayImporter(stockExchange.TradingCalander, dataService);

            _Scheduler = new Scheduler();
            _Scheduler.Add(ImportHistoricalPrices, Schedule.Daily().At(22, 00));
            _Scheduler.Add(ImportLivePrices, Schedule.Daily().Every(15, TimeUnit.Minutes).From(9, 30).To(17, 00));
            _Scheduler.Add(ImportTradingDays, Schedule.Monthly().On(Occurance.Last, Day.Friday).At(18, 00));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _Scheduler.Run(stoppingToken);
        }

        private void ImportHistoricalPrices()
        {
            var importTask = _HistoricalPriceImporter.Import(CancellationToken.None);
            importTask.Wait();
        }

        private void ImportLivePrices()
        {
            var importTask = _LivePriceImporter.Import(CancellationToken.None);
            importTask.Wait();
        }

        private void ImportTradingDays()
        {
            var importTask = _TradingDayImporter.Import(CancellationToken.None);
            importTask.Wait();
        }
    }
}
