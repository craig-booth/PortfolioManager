using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.DataImporter.DataServices
{
    public interface ILiveStockPriceService
    {
        Task<StockPrice> GetSinglePrice(string asxCode);
        Task<IEnumerable<StockPrice>> GetMultiplePrices(IEnumerable<string> asxCodes);
    }

    public interface IHistoricalStockPriceService
    {
        Task<IEnumerable<StockPrice>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate);
    }

    public interface ITradingDayService
    {
        Task<IEnumerable<DateTime>> NonTradingDays(int year);
    }

    public class StockPrice
    {
        public string ASXCode { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }

        public StockPrice()
        {

        }

        public StockPrice(string asxCode, DateTime date, decimal price)
        {
            ASXCode = asxCode;
            Date = date;
            Price = price;
        }
    }
}
