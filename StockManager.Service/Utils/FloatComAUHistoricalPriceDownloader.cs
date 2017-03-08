using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Globalization;

using KBCsv;

namespace StockManager.Service.Utils
{
    class FloatComAUHistoricalPriceDownloader : IHistoricalPriceDownloader
    {

        public async Task<IEnumerable<StockQuote>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate)
        {
            List<StockQuote> data = new List<StockQuote>();

            HttpWebRequest request;
            HttpWebResponse response;
            Stream dataStream;
            try
            {
                string url = string.Format("http://www.float.com.au/download/{0}.csv", asxCode);

                request = WebRequest.Create(url) as HttpWebRequest;
                response = await request.GetResponseAsync() as HttpWebResponse;

                dataStream = response.GetResponseStream();

                using (var textReader = new StreamReader(dataStream))
                {
                    using (var csvReader = new CsvReader(textReader, false))
                    {
                        var buffer = new DataRecord[128];
                        DateTime date;
                        decimal price;

                        while (csvReader.HasMoreRecords)
                        {
                            var read = await csvReader.ReadDataRecordsAsync(buffer, 0, buffer.Length);
                            for (var i = 0; i < read; ++i)
                            {
                                if (DateTime.TryParseExact(buffer[i][1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                    {                   
                                    if ((date >= fromDate) && (date <= toDate))
                                    {
                                        if (decimal.TryParse(buffer[i][5], out price))
                                            data.Add(new StockQuote(asxCode, date, price));
                                    }

                                }

                            }
                        }
                    }
                }

            }
            catch
            {
                return data;
            }

            return data;
        }
    }

}
