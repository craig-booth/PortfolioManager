using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using PortfolioManager.Common;

namespace PortfolioManager.DataImporter.DataServices
{
    class ASXDataService : ITradingDayService
    {
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
            var url = String.Format("http://www.asx.com.au/about/asx-trading-calendar-{0:d}.htm", year);
            var request = HttpWebRequest.CreateHttp(url);

            var response = (HttpWebResponse)await request.GetResponseAsync();

            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

            var text = await streamReader.ReadToEndAsync();

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
