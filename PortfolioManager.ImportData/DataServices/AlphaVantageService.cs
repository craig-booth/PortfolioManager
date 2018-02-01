using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.ImportData.DataServices
{
    public class AlphaVantageService : ILiveStockPriceService, IHistoricalStockPriceService
    {
        private const string _ApiKey = "KVFE5MLIMDUFO2TX";

        public async  Task<IEnumerable<StockPrice>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var result = new List<StockPrice>();

            try
            {
                var httpClient = new HttpClient();

                string url = String.Format("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={0}.AX&apikey={1}", asxCode, _ApiKey);
                var response = await httpClient.GetAsync(url);

                var text = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(text);

                result.AddRange(ParseResponce(jsonObject).Where(x => x.Date.Between(fromDate, toDate)).OrderBy(x => x.Date));                    
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IEnumerable<StockPrice>> GetMultiplePrices(IEnumerable<string> asxCodes)
        {
            var stockPrices = new List<StockPrice>();

            var tasks = asxCodes.Select(x => GetSinglePrice(x)).ToArray();

            Task.WaitAll(tasks);

            return tasks.Where(x => x.Result != null).Select(x => x.Result);
        }

        public async Task<StockPrice> GetSinglePrice(string asxCode)
        {
            try
            {
                var httpClient = new HttpClient();

                string url = String.Format("https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={0}.AX&apikey={1}", asxCode, _ApiKey);
                var response = await httpClient.GetAsync(url);

                var text = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(text);

                return ParseResponce(jsonObject).First();
            }
            catch
            {
                return null;
            }     
        }

        private IEnumerable<StockPrice> ParseResponce(JObject responce)
        {
            var symbol = (JValue)responce.SelectToken("['Meta Data']['2. Symbol']");
            var asxCode = (string)symbol.Value;
            if (asxCode.EndsWith(".AX"))
                asxCode = asxCode.Remove(asxCode.Length - 3);

            var dailyPrices = (JObject)responce.SelectToken("['Time Series (Daily)']");
            foreach (var dailyPrice in dailyPrices.Properties())
            {
                var date = DateTime.ParseExact(dailyPrice.Name, "yyyy-MM-dd", CultureInfo.CurrentCulture);

                var priceProperty = (JProperty)dailyPrice.Values().First(x => ((JProperty)x).Name == "4. close");
                var price = decimal.Parse((string)priceProperty.Value);

                yield return new StockPrice(asxCode, date, price);
            }
        }
    }
}
