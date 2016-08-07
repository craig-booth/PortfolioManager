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
    public class StockPrice
    {
        public Stock Stock { get; set; }
        public decimal Price { get; set; }

        public StockPrice(Stock stock)
            : this(stock, 0.00m)
        {

        }

        public StockPrice(Stock stock, decimal price)
        {
            Stock = stock;
            Price = price;
        }
    }


    public class StockPriceService
    {
        private readonly IStockQuery _StockQuery;
        private readonly IStockPriceDownloader _StockPriceDownloader;
        private Dictionary<string, StockQuote> _StockQuoteCache;

        internal StockPriceService(IStockQuery stockQuery, IStockPriceDownloader stockPriceDownloader)
        {
            _StockQuery = stockQuery;
            _StockPriceDownloader = stockPriceDownloader;
            _StockQuoteCache = new Dictionary<string, StockQuote>();
        }

        public decimal GetClosingPrice(Stock stock, DateTime date)
        {
            return _StockQuery.GetClosingPrice(stock.Id, date);
        }
      
        public decimal GetCurrentPrice(Stock stock)
        {
            StockQuote cachedStockQuote;

            if (!_StockQuoteCache.TryGetValue(stock.ASXCode, out cachedStockQuote))
            {
                cachedStockQuote = _StockPriceDownloader.GetSingleQuote(stock.ASXCode);
                if (cachedStockQuote == null)
                    return 0.00m;

                _StockQuoteCache.Add(cachedStockQuote.ASXCode, cachedStockQuote);
            }

            return cachedStockQuote.Price; 
        }

        public bool GetCurrentPrices(IEnumerable<StockPrice> prices)
        {
            // Try loading prices from cache 
            StockQuote cachedStockQuote;
            bool foundAll = true;
            foreach(var price in prices)
            {
                if (_StockQuoteCache.TryGetValue(price.Stock.ASXCode, out cachedStockQuote))
                {
                    price.Price = cachedStockQuote.Price;
                }
                else
                {
                    foundAll = false;
                    break;
                }
            }

            if (foundAll)
                return true;

            // If we didn't find them all then fetch all stock quotes
            var asxCodes = prices.Select(x => x.Stock.ASXCode);
            var stockQuotes = _StockPriceDownloader.GetMultipleQuotes(asxCodes);

            // Populate the prices in the array (and update cache)
            foreach (var price in prices)
            {
                var stockQuote = stockQuotes.FirstOrDefault(x => x.ASXCode == price.Stock.ASXCode);
                if (stockQuote != null)
                {
                    price.Price = stockQuote.Price;

                    // Update the cache
                    if (_StockQuoteCache.TryGetValue(price.Stock.ASXCode, out cachedStockQuote))
                    {
                        cachedStockQuote.Price = stockQuote.Price;
                        cachedStockQuote.Time = stockQuote.Time;
                    }
                    else
                        _StockQuoteCache.Add(stockQuote.ASXCode, stockQuote);
                }
            }

            return true;
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
