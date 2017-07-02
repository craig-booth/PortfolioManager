using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.DataImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = @"C:\PortfolioManager\Stocks.db";
            var database = new SQLiteStockDatabase(fileName);

            var tradingDayImporter = new TradingDayImporter(database);
            tradingDayImporter.Import().Wait();

            var livePriceImporter = new LivePriceImporter(database);
            livePriceImporter.Import().Wait();

            var historicalPriceImporter = new HistoricalPriceImporter(database);
            historicalPriceImporter.Import().Wait();

            /*      var scheduler = new ActionScheduler();
                  scheduler.AddRecurringAction(() => tradingDayImporter.Import(), DateTime.Now, new TimeSpan(0, 2, 0));
                  scheduler.AddRecurringAction(() => livePriceImporter.Import(), DateTime.Now, new TimeSpan(0, 0, 10));
                  scheduler.AddRecurringAction(() => historicalPriceImporter.Import(), DateTime.Now, new TimeSpan(0, 0, 30));

                  scheduler.Start();

                  Console.ReadKey();
                  scheduler.Stop(); 
                  Console.ReadKey(); */
        }

       
    }
}