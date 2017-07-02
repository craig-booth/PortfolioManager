using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PortfolioManager.DataImporter.DataServices
{
    class GoogleDataService : ILiveStockPriceService
    {
        private readonly JsonSerializerSettings _SerializerSettings;

        public GoogleDataService()
        {
            _SerializerSettings = new JsonSerializerSettings();
            _SerializerSettings.DateFormatString = "YYYY-MM-DD";
            _SerializerSettings.ContractResolver = new GoogleStockContractResolver();
        }

        public async Task<StockPrice> GetSinglePrice(string asxCode)
        {
            var quotes = await GetMultiplePrices(new string[] { asxCode });

            return quotes.FirstOrDefault();
        }

        public async Task<IEnumerable<StockPrice>> GetMultiplePrices(IEnumerable<string> asxCodes)
        {
            try
            {
                string url = "";
                foreach (var asxCode in asxCodes)
                {
                    if (url == "")
                        url += "https://www.google.com/finance/info?q=ASX:" + asxCode;
                    else
                        url += ",ASX:" + asxCode;
                }
                var request = WebRequest.Create(url) as HttpWebRequest;
                var response = await request.GetResponseAsync() as HttpWebResponse;

                var stream = response.GetResponseStream();
                var streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

                var text = await streamReader.ReadToEndAsync();

                // remove comment at start if needed
                var start = text.IndexOfAny(new char[] { '[', '{' });
                if (start == -1)
                    start = 0;

                return JsonConvert.DeserializeObject<StockPrice[]>(text.Substring(start), _SerializerSettings);
            }
            catch
            {
                return new StockPrice[0];
            }
        }

        public class GoogleStockContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                if (propertyName == "ASXCode")
                    return "t";
                else if (propertyName == "Price")
                    return "l";
                else if (propertyName == "Time")
                    return "lt_dts";
                else
                    return base.ResolvePropertyName(propertyName);
            }
        }

    }
}
