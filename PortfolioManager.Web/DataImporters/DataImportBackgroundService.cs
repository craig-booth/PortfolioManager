using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PortfolioManager.Common.Scheduler;

namespace PortfolioManager.Web.DataImporters
{
    public class DataImportBackgroundService : BackgroundService
    {
        private Scheduler _Scheduler;

        private readonly HistoricalPriceImporter _HistoricalPriceImporter;
        private readonly LivePriceImporter _LivePriceImporter;
        private readonly TradingDayImporter _TradingDayImporter;

        public DataImportBackgroundService(Scheduler scheduler, HistoricalPriceImporter historicalPriceImporter, LivePriceImporter livePriceImporter, TradingDayImporter tradingDayImporter)
        {
            _Scheduler = scheduler;
            _HistoricalPriceImporter = historicalPriceImporter;
            _LivePriceImporter = livePriceImporter;
            _TradingDayImporter = tradingDayImporter;

            _Scheduler.Add("Import Historical Prices", ImportHistoricalPrices, Schedule.Daily().At(20, 00));
            _Scheduler.Add("Import Live Prices", ImportLivePrices, Schedule.Daily().Every(5, TimeUnit.Minutes).From(9, 30).To(17, 00));
            _Scheduler.Add("Import Trading Days", ImportTradingDays, Schedule.Monthly().On(Occurance.Last, Day.Friday).At(18, 00));

    //        _Scheduler.Add("Import Historical Prices", ImportHistoricalPrices, Schedule.Daily().Every(1, TimeUnit.Minutes).From(9, 32).To(23, 00));
    //        _Scheduler.Add("Import Live Prices", ImportLivePrices, Schedule.Daily().Every(5, TimeUnit.Minutes).From(9, 30).To(23, 00));
    //        _Scheduler.Add("Import Trading Days", ImportTradingDays, Schedule.Monthly().On(Occurance.Last, Day.Friday).At(18, 00));
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
