using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.ImportData;
using PortfolioManager.ImportData.DataServices;

namespace PortfolioManager.Housekeeping
{
    class DownloadCommand : CommandLineApplication
    {
        private readonly CommandArgument _Database;
        private readonly CommandOption _TradingDays;
        private readonly CommandOption _CurrentPrices;
        private readonly CommandOption _HistoricPrices;

        public DownloadCommand()
        {

            Name = "download";
            Description = "Download data";

            _Database = Argument("database", "Database to update");
            _TradingDays = Option("-td | --tradingdays", "Download trading days for the current year", CommandOptionType.NoValue);
            _CurrentPrices = Option("-cp | --currentprices", "Download current price data", CommandOptionType.NoValue);
            _HistoricPrices = Option("-hp | --historicprices", "Download historic price data", CommandOptionType.NoValue);

            HelpOption("-? | -h | --help");

            OnExecute((Func<int>)RunCommand);
        }

        private int RunCommand()
        {
     /*       var downloadTasks = new List<Task>();

            var database = new SQLiteStockDatabase(_Database.Value);
   

            if (_TradingDays.HasValue())
            {
                var tradingDayImporter = new TradingDayImporter(database, new ASXDataService());
                downloadTasks.Add(tradingDayImporter.Import());
            }

            if (_CurrentPrices.HasValue())
            {
                var livePriceImporter = new LivePriceImporter(database, new ASXDataService());
                downloadTasks.Add(livePriceImporter.Import());
            }

            if (_HistoricPrices.HasValue())
            {
                var historicalPriceImporter = new HistoricalPriceImporter(database, new ASXDataService());
                downloadTasks.Add(historicalPriceImporter.Import());
            }

            Task.WaitAll(downloadTasks.ToArray());
            */
            return 0; 
        }
    }

}
