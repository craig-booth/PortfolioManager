using System;
using System.Linq;

using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.EventStore.Mongodb;

namespace PortfolioManager.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            //  AddDividends.Add();
            // CopyStore();
            // CheckStockDividendRules();

            // StockPriceHistoryTest.Test();

            SpliPriceHistoryFromStock();
        }

        public static void CheckStockDividendRules()
        {
            var check = new CheckStockDividendRules();
            var t = check.CheckAll();
            t.Wait();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static void CopyStore()
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            var sourceEventStore = new MongodbEventStore("mongodb://192.168.99.100:27017");
            var destinationEventStore = new MongodbEventStore("mongodb://portfolio.boothfamily.id.au:27017");

            EventStoreUtils.CopyEventStore(sourceEventStore, destinationEventStore);

      //      var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");
       //     EventStoreUtils.CreatePortfolio(destinationEventStore, portfolioId, "Test");

            Console.WriteLine("Done");
            Console.ReadKey(); 
        }

        public static void SpliPriceHistoryFromStock()
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            var eventStore = new MongodbEventStore("mongodb://portfolio.boothfamily.id.au:27017");
            EventStoreUtils.SplitPriceHistoryFromStock(eventStore);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

}
