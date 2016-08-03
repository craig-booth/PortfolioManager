using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service
{
    public class StockPriceService
    {
        private readonly IStockQuery _StockQuery;
        private readonly IStockPriceDownloader _StockPriceDownloader;

        internal StockPriceService(IStockQuery stockQuery, IStockPriceDownloader stockPriceDownloader)
        {
            _StockQuery = stockQuery;
            _StockPriceDownloader = stockPriceDownloader;
        }

        public decimal GetClosingPrice(Stock stock, DateTime date)
        {
            return _StockQuery.GetClosingPrice(stock.Id, date);
        }

        public decimal GetClosingPrice(string asxCode, DateTime date)
        {
            var stock = _StockQuery.GetByASXCode(asxCode, date);

            return _StockQuery.GetClosingPrice(stock.Id, date);
        }

        public decimal GetCurrentPrice(Stock stock)
        {
            return _StockPriceDownloader.GetCurrentPrice(stock.ASXCode);
        }

        public decimal GetCurrentPrice(string asxCode)
        {
            return _StockPriceDownloader.GetCurrentPrice(asxCode);
        }

        public IList<StockPrice> GetCurrentPrice(IEnumerable<string> asxCodes)
        { 
            return _StockPriceDownloader.GetMultipleQuotes(asxCodes);
        }

        public decimal GetPrice(Stock stock, DateTime date)
        {
            if (date == DateTime.Today)
                return GetCurrentPrice(stock);
            else
                return _StockQuery.GetClosingPrice(stock.Id, date);
        }

        public decimal GetPrice(string asxCode, DateTime date)
        {
            if (date == DateTime.Today)
                return GetCurrentPrice(asxCode);
            else
                return GetClosingPrice(asxCode, date);
        }

    }
}
