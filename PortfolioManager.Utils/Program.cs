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
            CopyStore();
            // CheckStockDividendRules();
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

            // var sourceEventStore = new MongodbEventStore("mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017");
       //     var sourceEventStore = new MongodbEventStore2("mongodb://192.168.99.100:27017");
            var destinationEventStore = new MongodbEventStore3("mongodb://192.168.99.100:27017");

            EventStoreUtils.CopyEventStore(destinationEventStore, destinationEventStore);

      //      var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");
       //     EventStoreUtils.CreatePortfolio(destinationEventStore, portfolioId, "Test");

            Console.WriteLine("Done");
            Console.ReadKey(); 
        }
    }

}
