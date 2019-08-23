using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.EventStore;
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

            // SpliPriceHistoryFromStock();

            TestEntityProperties();
        }


        public static void TestEntityProperties()
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            var eventStore = new MongodbEventStore("mongodb://portfolio.boothfamily.id.au:27017");
            var eventStream = eventStore.GetEventStream("Test");

            var id = Guid.NewGuid();

            var properties = new Dictionary<string, string>();
            properties.Add("UserName", "craig@boothfamily.id.au");
            properties.Add("Type", "Admin");

            var events = new List<Event>();
            eventStream.Add(id, "Test", properties, events);

            properties.Remove("Type");
            eventStream.UpdateProperties(id, properties);


            var x = eventStream.Find("UserName", "craig@boothfamily.id.au");

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static void TestEntityProperties2()
        {




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
