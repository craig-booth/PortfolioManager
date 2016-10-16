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
            decimal closingPrice;

            if (TryGetClosingPrice(stock, date, out closingPrice))
                return closingPrice;
            else
                return 0.00m;
        }

        public bool TryGetClosingPrice(Stock stock, DateTime date, out decimal price)
        {
            if (stock.ParentId == Guid.Empty)
            {
                return _StockQuery.TryGetClosingPrice(stock.Id, date, out price);
            }
            else
            {
                decimal parentPrice;
                if (_StockQuery.TryGetClosingPrice(stock.ParentId, date, out parentPrice))
                {
                    var percentOfPrice = _StockQuery.PercentOfParentCost(stock.ParentId, stock.Id, date);

                    price = parentPrice * percentOfPrice;
                    return true;
                }
                else
                {
                    price = 0.00m;
                    return false;
                }

            }
        }
      
        public decimal GetCurrentPrice(Stock stock)
        {
            var prices = new StockPrice[] { new StockPrice(stock)};

            GetCurrentPrices(prices);

            return prices[0].Price;
        }
        
        private bool GetPricesFromCache(IEnumerable<StockPrice> prices)
        {
            StockQuote cachedStockQuote;
            decimal percentOfPrice;
            string asxCode;

            bool allFound = true;
            foreach (var price in prices)
            {
                if (price.Stock.ParentId == Guid.Empty)
                {
                    asxCode = price.Stock.ASXCode;
                    percentOfPrice = 1.00m;
                }
                else
                {
                    var parentStock = _StockQuery.Get(price.Stock.ParentId, DateTime.Today);

                    asxCode = parentStock.ASXCode;
                    percentOfPrice = _StockQuery.PercentOfParentCost(parentStock.Id, price.Stock.Id, DateTime.Today);
                }

                if (_StockQuoteCache.TryGetValue(asxCode, out cachedStockQuote))
                    price.Price = cachedStockQuote.Price * percentOfPrice;
                else
                    allFound = false;
            }

            return allFound;
        }

        private void UpdateCache(IEnumerable<StockQuote> quotes)
        {
            StockQuote cachedStockQuote;

            foreach (var quote in quotes)
            {
                if (_StockQuoteCache.TryGetValue(quote.ASXCode, out cachedStockQuote))
                {
                    cachedStockQuote.Price = quote.Price;
                    cachedStockQuote.Time = quote.Time;
                }
                else
                    _StockQuoteCache.Add(quote.ASXCode, quote);
            }
        }

        public bool GetCurrentPrices(IEnumerable<StockPrice> prices)
        {
            // Try loading prices from cache 
            if (GetPricesFromCache(prices))
                return true;

            // If we didn't find them all then fetch all stock quotes
            var asxCodes = new List<string>();
            foreach (var price in prices)
            {
                if (price.Stock.ParentId == Guid.Empty)
                    asxCodes.Add(price.Stock.ASXCode);
                else
                {
                    var parentStock = _StockQuery.Get(price.Stock.ParentId, DateTime.Today);
                    if (!asxCodes.Contains(parentStock.ASXCode))
                        asxCodes.Add(parentStock.ASXCode);
                }
            }
            var stockQuotes = _StockPriceDownloader.GetMultipleQuotes(asxCodes);

            // Update the cache with the returned quotes
            UpdateCache(stockQuotes);

            // Try to fetch from the cache again
            return GetPricesFromCache(prices);
        } 

        public decimal GetPrice(Stock stock, DateTime date)
        {
            if (date == DateTime.Today)
                return GetCurrentPrice(stock);
            else
                return GetClosingPrice(stock, date);
        }

    }
}
