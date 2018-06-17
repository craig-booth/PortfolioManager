using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PortfolioManager.Common.Scheduler;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Web
{
    public class DataImportBackgroundService : BackgroundService
    {
        private Scheduler _Scheduler;

        private readonly ILogger _Logger;
        private readonly HistoricalPriceImporter _HistoricalPriceImporter;
        private readonly LivePriceImporter _LivePriceImporter;
        private readonly TradingDayImporter _TradingDayImporter;

        public DataImportBackgroundService(StockExchange stockExchange, IHistoricalStockPriceService historicalStockPriceService, ILiveStockPriceService liveStockPriceService, ITradingDayService tradingDayService,  ILogger<DataImportBackgroundService> logger)
        {
            _Logger = logger;

            _HistoricalPriceImporter = new HistoricalPriceImporter(stockExchange, historicalStockPriceService, _Logger);
            _LivePriceImporter = new LivePriceImporter(stockExchange, liveStockPriceService, _Logger);
            _TradingDayImporter = new TradingDayImporter(stockExchange.TradingCalander, tradingDayService, _Logger);

            _Scheduler = new Scheduler(_Logger);
            //        _Scheduler.Add("Import Historical Prices", ImportHistoricalPrices, Schedule.Daily().At(20, 00));
            //        _Scheduler.Add("Import Live Prices", ImportLivePrices, Schedule.Daily().Every(5, TimeUnit.Minutes).From(9, 30).To(17, 00));
            //        _Scheduler.Add("Import Trading Days", ImportTradingDays, Schedule.Monthly().On(Occurance.Last, Day.Friday).At(18, 00));

            _Scheduler.Add("Import Historical Prices", ImportHistoricalPrices, Schedule.Daily().Every(5, TimeUnit.Minutes).From(9, 32).To(23, 00));
            _Scheduler.Add("Import Live Prices", ImportLivePrices, Schedule.Daily().Every(5, TimeUnit.Minutes).From(9, 30).To(23, 00));
    //        _Scheduler.Add("Import Trading Days", ImportTradingDays, Schedule.Monthly().On(Occurance.Last, Day.Friday).At(18, 00));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _Logger.LogInformation("Starting scheduler...");
            try
            {
                await _Scheduler.Run(stoppingToken);
            }
            catch (Exception e)
            {
                _Logger.LogError(e, "Error occurred in Scheduler");
            }
        }

        private void ImportHistoricalPrices()
        {
            try
            {
                var importTask = _HistoricalPriceImporter.Import(CancellationToken.None);
                importTask.Wait();
            }
            catch (Exception e)
            {
                _Logger.LogError(e, "Error occurred importing historical prices");
                return;
            }
            _Logger.LogInformation("{0} - Historical prices imported", DateTime.Now);
        }

        private void ImportLivePrices()
        {
            try
            {
                var importTask = _LivePriceImporter.Import(CancellationToken.None);
                importTask.Wait();
            }
            catch (Exception e)
            {
                _Logger.LogError(e, "Error occurred importing live prices");
                return;
            }
            _Logger.LogInformation("{0} - Live prices imported", DateTime.Now);
        }

        private void ImportTradingDays()
        {
            try
            {
                var importTask = _TradingDayImporter.Import(CancellationToken.None);
                importTask.Wait();
            }
            catch (Exception e)
            {
                _Logger.LogError(e, "Error occurred import trading days");
                return;
            }
            _Logger.LogInformation("{0} Trading days imported", DateTime.Now);
        }
    }
}
