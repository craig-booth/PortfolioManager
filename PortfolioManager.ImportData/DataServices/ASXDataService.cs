using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.TradingCalanders;

namespace PortfolioManager.ImportData.DataServices
{
    public class ASXDataService : ITradingDayService, ILiveStockPriceService, IHistoricalStockPriceService
    {
        public async Task<IEnumerable<StockPrice>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            var result = new List<StockPrice>();

            try
            {
                var httpClient = new HttpClient();

                string url = String.Format("https://www.asx.com.au/asx/1/share/{0}/prices?interval=daily&count={1}", asxCode, toDate.Subtract(fromDate).Days);
                var response = await httpClient.GetAsync(url, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    var text = await response.Content.ReadAsStringAsync();

                    var jsonObject = JObject.Parse(text);

                    var closingPrices = from item in jsonObject["data"]
                                        select new StockPrice(asxCode, ((DateTime)item["close_date"]).AddHours(2).Date, (decimal)item["close_price"]);

                    result.AddRange(closingPrices.Where(x => x.Date.Between(fromDate, toDate)).OrderBy(x => x.Date));
                }
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IEnumerable<StockPrice>> GetMultiplePrices(IEnumerable<string> asxCodes, CancellationToken cancellationToken)
        {
            var stockPrices = new List<StockPrice>();

            var tasks = asxCodes.Select(x => GetSinglePrice(x, cancellationToken)).ToArray();

            Task.WaitAll(tasks, cancellationToken);

            return tasks.Where(x => x.Result != null).Select(x => x.Result);  
        }

        public async Task<StockPrice> GetSinglePrice(string asxCode, CancellationToken cancellationToken)
        {
            
            try
            {
                var httpClient = new HttpClient();

                string url = "https://www.asx.com.au/asx/1/share/" + asxCode;         
                var response = await httpClient.GetAsync(url, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    return null;

                var text = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(text);

                var errorCode = jsonObject.SelectToken("error_code");

                var price = (decimal)jsonObject.SelectToken("last_price");
                var date = (DateTime)jsonObject.SelectToken("last_trade_date");

                return new StockPrice(asxCode, date.AddHours(2).Date, price);

            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<NonTradingDay>> NonTradingDays(int year, CancellationToken cancellationToken)
        {
            var days = new List<NonTradingDay>();

            var data = await DownloadData(year, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return days;

            foreach (var tableRow in data.Descendants("tr"))
            {
                var nonTradingDay = ParseRow(tableRow as XElement, year);
                if (nonTradingDay != null)
                    days.Add(nonTradingDay);
            }

            return days;
        }

        private async Task<XElement> DownloadData(int year, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();

            var url = String.Format("http://www.asx.com.au/about/asx-trading-calendar-{0:d}.htm", year);
            var response = await httpClient.GetAsync(url, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return null;

            var text = await response.Content.ReadAsStringAsync();
           
            // Find start of data
            var start = text.IndexOf("<!-- start content -->");
            var end = -1;
            if (start >= 0)
            {
                start = text.IndexOf("<tbody>", start);
                end = text.IndexOf("</tbody>", start);
            }

            if ((start >= 0) && (end >= 0))
            {
                var data = text.Substring(start, end - start + 8);

                data = data.Replace("&nbsp;", " ");

                return XElement.Parse(data);
            }
            else 
                return null;
        }

        private NonTradingDay ParseRow(XElement row, int year)
        {
            DateTime date = DateUtils.NoDate;

            var cells = row.Descendants("td").ToList();
            if (cells.Count >= 4)
            {              
                if (GetCellValue(cells[3])== "CLOSED")
                {
                    var description = GetCellValue(cells[0]);

                    var dateText = GetCellValue(cells[1]) + " " + year;
                    if (DateTime.TryParse(dateText, out date))
                        return new NonTradingDay(date, description);
                }
            }

            return null;
        }

        private string GetCellValue(XElement cell)
        {
            var sups = cell.Descendants("sup").ToArray();
            for (var i = 0; i < sups.Length; i++)
                sups[i].Remove();

            return cell.Value.Trim();
        }

    }
}
