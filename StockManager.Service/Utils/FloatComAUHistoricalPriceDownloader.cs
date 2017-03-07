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
    class FloatComAUHistoricalPriceDownloader : IHistoricalPriceDownloader
    {

        public async Task<List<StockQuote>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate)
        {
            List<StockQuote> data = new List<StockQuote>();

            HttpWebRequest request;
            HttpWebResponse response;
            Stream dataStream;
            StreamReader reader;
            try
            {
                string url = string.Format("http://www.float.com.au/download/{0}.csv", asxCode);

                request = WebRequest.Create(url) as HttpWebRequest;
                response = await request.GetResponseAsync() as HttpWebResponse;

                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);

                var csvData = new MemoryStream();
                var writer = new StreamWriter(csvData);
                char[] buffer = new char[32768];
                int bytesRead;
                while ((bytesRead = await reader.ReadAsync(buffer, 0, 32768)) > 0)
                {
                    await writer.WriteAsync(buffer, 0, bytesRead);
                }


                var csvReader = new CsvReader(new StreamReader(csvData));

                csvReader.Configuration.RegisterClassMap(new FloatComAUClassMap());
                csvReader.Configuration.HasHeaderRecord = false;
                while (csvReader.Read())
                {
                    var quote = csvReader.GetRecord<StockQuote>();
                    if ((quote.Date >= fromDate) && (quote.Date <= toDate))
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

    class FloatComAUClassMap : CsvClassMap<StockQuote>
    {
        public FloatComAUClassMap()
        {
            Map(x => x.ASXCode).Index(0);
            Map(x => x.Date).Index(1).TypeConverterOption("yyyyMMdd");
            Map(x => x.Price).Index(5);
        }
    }

}
