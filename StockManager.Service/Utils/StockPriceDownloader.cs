using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StockManager.Service.Utils
{

    public class StockQuote
    {
        public string ASXCode { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }

        public StockQuote()
        {

        }

        public StockQuote(string asxCode, DateTime date, decimal price)
        {
            ASXCode = asxCode;
            Date = date;
            Price = price;
        }
    }


    public interface IStockPriceDownloader
    {
        StockQuote GetSingleQuote(string asxCode);
        IEnumerable<StockQuote> GetMultipleQuotes(IEnumerable<string> asxCodes);
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

        public StockQuote GetSingleQuote(string asxCode)
        {
            var quotes = GetMultipleQuotes(new string[] { asxCode });

            return quotes.FirstOrDefault();
        }

        public IEnumerable<StockQuote> GetMultipleQuotes(IEnumerable<string> asxCodes)
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

                return JsonConvert.DeserializeObject<StockQuote[]>(responseFromServer.Substring(start), _SerializerSettings);
            }
            catch
            {
                return new StockQuote[0];
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
