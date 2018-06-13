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

            _HistoricalPriceImporter = new HistoricalPriceImporter(stockExchange, historicalStockPriceService);
            _LivePriceImporter = new LivePriceImporter(stockExchange, liveStockPriceService);
            _TradingDayImporter = new TradingDayImporter(stockExchange.TradingCalander, tradingDayService);

            _Scheduler = new Scheduler();
            _Scheduler.Add(ImportHistoricalPrices, Schedule.Daily().At(22, 00));
            _Scheduler.Add(ImportLivePrices, Schedule.Daily().Every(5, TimeUnit.Minutes).From(9, 30).To(17, 00));
            _Scheduler.Add(ImportTradingDays, Schedule.Monthly().On(Occurance.Last, Day.Friday).At(18, 00));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _Scheduler.Run(stoppingToken);
        }

        private void ImportHistoricalPrices()
        {
            _Logger.LogInformation("{0} Importing historical prices", DateTime.Now);
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
            _Logger.LogInformation("Import of historical prices complete");
        }

        private void ImportLivePrices()
        {
            _Logger.LogInformation("{0} Importing live prices", DateTime.Now);
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
            _Logger.LogInformation("Import of live prices complete");
        }

        private void ImportTradingDays()
        {
            _Logger.LogInformation("{0} Importing trading days", DateTime.Now);
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
            _Logger.LogInformation("Import of trading days complete");
        }
    }
}
