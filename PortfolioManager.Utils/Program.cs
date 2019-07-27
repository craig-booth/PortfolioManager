using System;
using System.Linq;

using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.Common;

namespace PortfolioManager.Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            //  AddDividends.Add();
            //    CopyStore();
            // CheckStockDividendRules();

            //AddStock();
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

        public static void AddStock()
        {
            var url = "https://portfolio.boothfamily.id.au";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
            var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");

            var restClient = new RestClient(url, apiKey, portfolioId);

            var command = new CreateStockCommand()
            {
                Id = new Guid(),
                ListingDate = new DateTime(2018, 10, 24),
                AsxCode = "VISM",
                Name = "Vanguard International Small Caps ETF",
                Trust = true,
                Category = AssetCategory.InternationalStocks
            };
            var request = restClient.Stocks.CreateStock(command);
            request.Wait();
        }
    }

}
