using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.Service.Utils
{
    public interface IStockPriceDownloader
    {
        StockQuote GetSingleQuote(string asxCode);
        IEnumerable<StockQuote> GetMultipleQuotes(IEnumerable<string> asxCodes);
    }

    public interface IHistoricalPriceDownloader
    {
        Task<List<StockQuote>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate);
    }

    public interface ITradingDayDownloader
    {
        Task<List<DateTime>> NonTradingDays(int year);
      
    }

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

}
