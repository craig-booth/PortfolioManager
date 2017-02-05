using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;

using PortfolioManager.Model.Utils;

namespace StockManager.Service.Utils
{
 
    class TradingDayDownloader
    {
        public static IEnumerable<DateTime> NonTradingDays(int year)
        {
            var data = DownloadData(year);

            foreach (var tableRow in data.ChildNodes)
            {
                var date = ParseRow(tableRow as XmlElement, year);
                if (date != DateUtils.NoDate)
                    yield return date;
            }

        }

        private static DateTime ParseRow(XmlElement row, int year)
        {
            if ((row.Name == "tr") && (row.ChildNodes.Count >= 4))
            {
                var dateText = row.ChildNodes[1].FirstChild.InnerText + " " + year;
                var statusText = row.ChildNodes[3].FirstChild.InnerText.Trim();

                if (statusText == "CLOSED")
                {
                    DateTime date;

                    if (DateTime.TryParse(dateText, out date))
                        return date;
                }
            }
            
            return DateUtils.NoDate;        
        }

        private static XmlElement DownloadData(int year)
        {
            var url = String.Format("http://www.asx.com.au/about/asx-trading-calendar-{0:d}.htm", year);
            var request = HttpWebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();


            var responseStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8"));

            var responseText = responseStream.ReadToEnd();

            responseStream.Close();
            response.Close();

            // Find start of data
            var start = responseText.IndexOf("<!-- start content -->");
            var end = -1;
            if (start >= 0)
            {
                start = responseText.IndexOf("<tbody>", start);
                end = responseText.IndexOf("</tbody>", start);
            }

            if ((start >= 0) && (end >= 0))
            {
                var data = responseText.Substring(start, end - start + 8);

                data = data.Replace("&nbsp;", " ");

                var xml = new XmlDocument();
                xml.LoadXml(data);

                return xml.DocumentElement;
            }
            else
                return null;

        }
    }
}
