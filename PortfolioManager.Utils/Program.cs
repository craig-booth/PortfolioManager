using System;

using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.EventStore.Mongodb;

namespace PortfolioManager.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ensure that PortfolioManager.Domain assembly is loaded
            var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            var sourceEventStore = new MongodbEventStore("mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017");
            var destinationEventStore = new MongodbEventStore2("mongodb://192.168.99.100:27017");

            EventStoreUtils.CopyEventStore(sourceEventStore, destinationEventStore);

            var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");
            EventStoreUtils.CreatePortfolio(destinationEventStore, portfolioId, "Test");

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

}
