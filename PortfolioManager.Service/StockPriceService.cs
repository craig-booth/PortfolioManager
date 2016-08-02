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

        public decimal GetCurrentPrice(Stock stock)
        {
            var price = _StockPriceDownloader.GetCurrentPrice(stock.ASXCode);

            if (price == 0.00m)
                price = GetClosingPrice(stock, DateTime.Today.AddDays(-1));

            return price;
        }

        public decimal GetPrice(Stock stock, DateTime date)
        {
            if (date == DateTime.Today)
                return GetCurrentPrice(stock);
            else
                return _StockQuery.GetClosingPrice(stock.Id, date);
        }

    }
}
