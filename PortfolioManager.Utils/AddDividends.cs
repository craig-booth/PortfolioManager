using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.CorporateActions;

namespace PortfolioManager.Utils
{
    static class AddDividends
    {

        public static async void Add()
        {
            var url = "https://docker.local:8443";
            //   var url = "http://localhost";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
            var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");

        //    var restClient = new RestClient(url, apiKey, portfolioId);

          //  var t = AddAll(restClient);
        //    t.Wait();
        }

        private static async Task AddAll(RestClient restClient)
        {
            Guid stockId;

     /*       stockId = await GetStockId(restClient, "ARG");
            await AddDividend(restClient, stockId, new DateTime(2018, 02, 19), new DateTime(2018, 03, 09), 0.155m, 1.0m, 7.98m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 08, 27), new DateTime(2018, 09, 14), 0.16m, 1.0m, 7.89m, "");
            await AddDividend(restClient, stockId, new DateTime(2019, 02, 18), new DateTime(2019, 03, 08), 0.16m, 1.0m, 7.53m, "");

            stockId = await GetStockId(restClient, "COH");
            await AddDividend(restClient, stockId, new DateTime(2018, 03, 20), new DateTime(2018, 04, 12), 1.40m, 1.0m, 0.0m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 09, 18), new DateTime(2018, 10, 10), 1.60m, 1.0m, 0.0m, "");

            stockId = await GetStockId(restClient, "CSL");
            await AddDividend(restClient, stockId, new DateTime(2018, 03, 15), new DateTime(2018, 04, 13), 1.004959m, 0.0m, 0.0m, "Dividend US$0.79. This dividend is converted to AU currency at the rate of US$1.00 = AU$1.2721");
            await AddDividend(restClient, stockId, new DateTime(2018, 09, 12), new DateTime(2018, 10, 12), 1.278192m, 1.0m, 0.0m, "Dividend US$0.93. This dividend is converted to AU currency at the rate of US$1.00 = AU$1.3744");
            */
          //  stockId = await GetStockId(restClient, "GVF");
          //  await AddDividend(restClient, stockId, new DateTime(2018, 04, 10), new DateTime(2018, 05, 10), 0.0315m, 0.50m, 1.07284m, "");
          //  await AddDividend(restClient, stockId, new DateTime(2018, 10, 01), new DateTime(2018, 11, 09), 0.0315m, 0.70m, 1.0817m, "");
/*
            stockId = await GetStockId(restClient, "VAF");
            await AddDividend(restClient, stockId, new DateTime(2018, 04, 04), new DateTime(2018, 04, 18), 0.34136806m, 0.0m, 48.5492m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 07, 03), new DateTime(2018, 07, 17), 0.36453220m, 0.0m, 48.5508m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 10, 02), new DateTime(2018, 10, 16), 0.26433233m, 0.0m, 48.5264m, "");
            await AddDividend(restClient, stockId, new DateTime(2019, 01, 03), new DateTime(2019, 01, 17), 0.35898295m, 0.0m, 49.2433m, "");

            stockId = await GetStockId(restClient, "VAP");
            await AddDividend(restClient, stockId, new DateTime(2018, 04, 04), new DateTime(2018, 04, 18), 1.14982467m, 0.0m, 77.0961m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 06, 06), new DateTime(2018, 06, 14), 4.48937186m, 0.0m, 79.1966m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 07, 03), new DateTime(2018, 07, 17), 0.52677060m, 0.0m, 79.5272m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 10, 02), new DateTime(2018, 10, 16), 0.32502769m, 0.0m, 80.7262m, "");
            await AddDividend(restClient, stockId, new DateTime(2019, 01, 03), new DateTime(2019, 01, 17), 1.36865757m, 0.0m, 78.0111m, "");

            stockId = await GetStockId(restClient, "VCF");
            await AddDividend(restClient, stockId, new DateTime(2018, 04, 04), new DateTime(2018, 04, 18), 0.31690507m, 0.0m, 47.5644m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 07, 03), new DateTime(2018, 07, 17), 0.85534103m, 0.0m, 46.6130m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 10, 02), new DateTime(2018, 10, 16), 0.21614463m, 0.0m, 46.6479m, "");
            await AddDividend(restClient, stockId, new DateTime(2019, 01, 03), new DateTime(2019, 01, 17), 0.21875182m, 0.0m, 46.6379m, "");


            stockId = await GetStockId(restClient, "VGE");
            await AddDividend(restClient, stockId, new DateTime(2018, 04, 04), new DateTime(2018, 04, 18), 0.05359180m, 0.0m, 68.5021m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 07, 03), new DateTime(2018, 07, 17), 0.28283567m, 0.0m, 63.9000m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 10, 02), new DateTime(2018, 10, 16), 0.55200837m, 0.0m, 63.4104m, "");
            await AddDividend(restClient, stockId, new DateTime(2019, 01, 03), new DateTime(2019, 01, 17), 0.31938848m, 0.0m, 60.5456m, "");


            stockId = await GetStockId(restClient, "VGS");
            await AddDividend(restClient, stockId, new DateTime(2018, 04, 04), new DateTime(2018, 04, 18), 0.20967478m, 0.0m, 65.7512m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 07, 03), new DateTime(2018, 07, 17), 0.90857336m, 0.0m, 68.5355m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 10, 02), new DateTime(2018, 10, 16), 0.30354890m, 0.0m, 73.2747m, "");
            await AddDividend(restClient, stockId, new DateTime(2019, 01, 03), new DateTime(2019, 01, 17), 0.39028555m, 0.0m, 64.7719m, "");

            stockId = await GetStockId(restClient, "WAM");
            await AddDividend(restClient, stockId, new DateTime(2018, 04, 13), new DateTime(2018, 04, 27), 0.0775m, 1.0m, 2.3502m, "");
            await AddDividend(restClient, stockId, new DateTime(2018, 11, 19), new DateTime(2018, 11, 26), 0.0775m, 1.0m, 2.09442m, "");
            */
        }

        private static async Task AddDividend(RestClient restClient, Guid stock, DateTime recordDate, DateTime paymentDate, decimal amount, decimal precentFranked, decimal drpPrice, string description)
        {
            var dividend = new Dividend()
            {
                Id = Guid.NewGuid(),
                Stock = stock,
                ActionDate = recordDate,
                Description = description,
                PaymentDate = paymentDate,
                DividendAmount = amount,
                PercentFranked = precentFranked,
                DRPPrice = drpPrice
            };

            await restClient.CorporateActions.Add(stock, dividend);
        }

        private static async Task<Guid> GetStockId(RestClient restClient, string asxCode)
        {
            var response = await restClient.Stocks.Find(asxCode);

            return response.First(x => x.ASXCode == asxCode).Id;
        }


    }
}
