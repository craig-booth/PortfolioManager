using System;
using System.Threading.Tasks;

using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.ImportData;

namespace PortfolioManager.Housekeeping
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Importing Data...");
            ImportData();
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        static async void ImportData()
        {
            var fileName = @"C:\PortfolioManager\Stocks.db";
            var database = new SQLiteStockDatabase(fileName);

            var tradingDayImporter = new TradingDayImporter(database);
            var livePriceImporter = new LivePriceImporter(database);
            var historicalPriceImporter = new HistoricalPriceImporter(database);


            await tradingDayImporter.Import();
            await livePriceImporter.Import();
            await historicalPriceImporter.Import();
        }
    }
}