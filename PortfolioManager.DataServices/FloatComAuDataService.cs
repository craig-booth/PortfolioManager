﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Globalization;
using KBCsv;

namespace PortfolioManager.DataServices
{
    public class FloatComAuDataService : IHistoricalStockPriceService
    {
        public async Task<IEnumerable<StockPrice>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            List<StockPrice> data = new List<StockPrice>();
            try
            {
                var httpClient = new HttpClient();

                string url = string.Format("http://www.float.com.au/download/{0}.csv", asxCode);
                var response = await httpClient.GetAsync(url, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return data;

                var dataStream = await response.Content.ReadAsStreamAsync();
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
                                            data.Add(new StockPrice(asxCode, date, price));
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
