using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;

namespace StockManager.Service.Utils
{

    public interface IHistoricalPriceDownloader
    {
        IEnumerable<StockQuote> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate);
    }

    class YahooHistoricalPriceDownloader: IHistoricalPriceDownloader
    {
        public IEnumerable<StockQuote> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate)
        {
            List<StockQuote> data = new List<StockQuote>();

            HttpWebRequest request;
            HttpWebResponse response;
            Stream dataStream;
            StreamReader reader;
            try
            {
                string url = string.Format("http://ichart.finance.yahoo.com/table.csv?s={0}.AX&a={1:d}&b={2:d}&c={3:d}&d={4:d}&e={5:d}&f={6:d}&g=d&ignore=.csv",
                    asxCode,
                    fromDate.Month, fromDate.Day, fromDate.Year,
                    toDate.Month, toDate.Day, toDate.Year
                    );

                request = WebRequest.Create(url) as HttpWebRequest;
                response = request.GetResponse() as HttpWebResponse;

                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);

                var csvReader = new CsvReader(reader);

                csvReader.Configuration.RegisterClassMap(new YahooFinianceClassMap());
                csvReader.Configuration.HasHeaderRecord = true;
                while (csvReader.Read())
                {
                    var quote = csvReader.GetRecord<StockQuote>();
                    quote.ASXCode = asxCode;
                    data.Add(quote);
                }

            }
            catch
            {
                return data;
            }

            return data;
        }
    }

    class YahooFinianceClassMap : CsvClassMap<StockQuote>
    {
        public YahooFinianceClassMap()
        {
            Map(x => x.Date).Index(0).TypeConverterOption("yyyy-MM-dd");
            Map(x => x.Price).Index(4);
        }
    }

}
