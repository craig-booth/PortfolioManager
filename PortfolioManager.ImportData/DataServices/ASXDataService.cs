using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.ImportData.DataServices
{
    class ASXDataService : ITradingDayService, IHistoricalStockPriceService, ILiveStockPriceService
    {
        public Task<IEnumerable<StockPrice>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StockPrice>> GetMultiplePrices(IEnumerable<string> asxCodes)
        {
            var stockPrices = new List<StockPrice>();

            foreach (var asxCode in asxCodes)
            {
                var stockPrice = await GetSinglePrice(asxCode);

                if (stockPrice != null)
                    stockPrices.Add(stockPrice);
            }

            return stockPrices;
        }

        public async Task<StockPrice> GetSinglePrice(string asxCode)
        {
            
            try
            {
                var httpClient = new HttpClient();

                string url = "http://data.asx.com.au/data/1/share/" + asxCode;         
                var response = await httpClient.GetAsync(url);

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

        public async Task<IEnumerable<DateTime>> NonTradingDays(int year)
        {
            var days = new List<DateTime>();

            var data = await DownloadData(year);

            foreach (var tableRow in data.Descendants("tr"))
            {
                var date = ParseRow(tableRow as XElement, year);
                if (date != DateUtils.NoDate)
                    days.Add(date);
            }

            return days;
        }

        private async Task<XElement> DownloadData(int year)
        {
            var httpClient = new HttpClient();

            var url = String.Format("http://www.asx.com.au/about/asx-trading-calendar-{0:d}.htm", year);
            var response = await httpClient.GetAsync(url);

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

        private DateTime ParseRow(XElement row, int year)
        {
            DateTime date = DateUtils.NoDate;

            var cells = row.Descendants("td").ToList();
            if (cells.Count >= 4)
            {              
                if (cells[3].Value.Trim() == "CLOSED")
                {                   
                    var dateText = cells[1].Value + " " + year;
                    DateTime.TryParse(dateText, out date);
                }
            }

            return date;
        }

    }
}
