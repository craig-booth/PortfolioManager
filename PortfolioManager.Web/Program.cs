using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using PortfolioManager.Common;
using PortfolioManager.ImportData;
using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ScheduleImports();

            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseKestrel() //opts => opts.Listen(IPAddress.Any, 5001))
                .Build();

            host.Run();          
        }

    /*    public static void ScheduleImports()
        {
            var fileName = @"C:\PortfolioManager\Stocks.db";
            var database = new SQLiteStockDatabase(fileName);

            var tradingDayImporter = new TradingDayImporter(database);
            var livePriceImporter = new LivePriceImporter(database);
            var historicalPriceImporter = new HistoricalPriceImporter(database);

            var scheduler = new ActionScheduler();
            scheduler.AddRecurringAction(() => tradingDayImporter.Import(), DateTime.Now, new TimeSpan(24, 0, 0));
            scheduler.AddRecurringAction(() => livePriceImporter.Import(), DateTime.Now, new TimeSpan(0, 5, 0));
            scheduler.AddRecurringAction(() => historicalPriceImporter.Import(), DateTime.Now, new TimeSpan(24, 0, 0));

            scheduler.Start();
        } */
    }
}
