using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioManager.ImportData.DataServices
{
    public interface ILiveStockPriceService
    {
        Task<StockPrice> GetSinglePrice(string asxCode, CancellationToken cancellationToken);
        Task<IEnumerable<StockPrice>> GetMultiplePrices(IEnumerable<string> asxCodes, CancellationToken cancellationToken);
    }

    public interface IHistoricalStockPriceService
    {
        Task<IEnumerable<StockPrice>> GetHistoricalPriceData(string asxCode, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
    }

    public interface ITradingDayService
    {
        Task<IEnumerable<DateTime>> NonTradingDays(int year, CancellationToken cancellationToken);
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
