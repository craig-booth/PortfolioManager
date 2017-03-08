using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.Service.Utils
{
    public interface IStockPriceDownloader
    {
        Task<StockQuote> GetSingleQuote(string asxCode);
        Task<IEnumerable<StockQuote>> GetMultipleQuotes(IEnumerable<string> asxCodes);
    }

    public interface IHistoricalPriceDownloader
    {
        Task<IEnumerable<StockQuote>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate);
    }

    public interface ITradingDayDownloader
    {
        Task<IEnumerable<DateTime>> NonTradingDays(int year);
      
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
