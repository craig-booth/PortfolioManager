using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Xml;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Test
{

    public class DownloadedDividendRecord
    {
        public string ASXCode;
        public DateTime RecordDate;
        public DateTime PaymentDate;
        public decimal Amount;
        public decimal PercentFranked;
    }

    public class DownloadService
    {
        public IEnumerable<DownloadedDividendRecord> DownloadDividendHistory(string asxCode)
        {
            //  Load Web page 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.sharedividends.com.au/index.php");

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream requestStream = request.GetRequestStream();

            StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.ASCII);
            string postData = "stock=" + asxCode + "&page=history";
            streamWriter.Write(postData, 0, postData.Length);
            streamWriter.Close();

            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
            string htmlPage = streamReader.ReadToEnd();

            streamReader.Close();
            response.Close();

            // Parse html page
            int start = htmlPage.IndexOf("<table>");
            int end = htmlPage.IndexOf("</table>", start);

            string table = htmlPage.Substring(start, end - start + 8);

            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(table);

            XmlNodeList tableRows = xmlData.SelectNodes("table/tr");
            foreach (XmlNode row in tableRows)
            {
                string recordDate = row.ChildNodes[4].InnerText;
                string paymentDate = row.ChildNodes[5].InnerText;
                string amount = row.ChildNodes[1].InnerText;
                string frankingRate = row.ChildNodes[2].InnerText;
                char[] charsToTrim = { '%' };
                frankingRate = frankingRate.TrimEnd(charsToTrim);

                yield return new DownloadedDividendRecord()
                {
                    ASXCode = asxCode,
                    RecordDate = DateTime.Parse(recordDate),
                    PaymentDate = DateTime.Parse(paymentDate),
                    Amount = Decimal.Parse(amount),
                    PercentFranked = Decimal.Parse(frankingRate) / 100
                };
            }
        }
    }
}
