using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using PortfolioManager.Model.Stocks;


namespace PortfolioManager.Service.Utils
{

    interface IStockPriceDownloader
    {
        decimal GetCurrentPrice(string asxCode);
        StockPrice GetSingleQuote(string asxCode);
        IList<StockPrice> GetMultipleQuotes(IEnumerable<string> asxCodes);
    }

    class GoogleStockPriceDownloader : IStockPriceDownloader
    {
        private readonly JsonSerializerSettings _SerializerSettings;

        public GoogleStockPriceDownloader()
        {
            _SerializerSettings = new JsonSerializerSettings();
            _SerializerSettings.DateFormatString = "YYYY-MM-DD";
            _SerializerSettings.ContractResolver = new GoogleStockContractResolver();
        }

        public decimal GetCurrentPrice(string asxCode)
        {
            return GetSingleQuote(asxCode).Price;
        }

        public StockPrice GetSingleQuote(string asxCode)
        {
            var quotes = GetMultipleQuotes(new string[] { asxCode });

            return quotes.FirstOrDefault();
        }

        public IList<StockPrice> GetMultipleQuotes(IEnumerable<string> asxCodes)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream dataStream;
            StreamReader reader;
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
                request = WebRequest.Create(url) as HttpWebRequest;
                response = request.GetResponse() as HttpWebResponse;

                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);

                var responseFromServer = reader.ReadToEnd();

                // remove comment at start if needed
                var start = responseFromServer.IndexOfAny(new char[] { '[', '{' });
                if (start == -1)
                    start = 0;

                return JsonConvert.DeserializeObject<StockPrice[]>(responseFromServer.Substring(start), _SerializerSettings);
            }
            catch
            {
                return new StockPrice[] { new StockPrice("", new DateTime(), 0.00m) };
            }
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
