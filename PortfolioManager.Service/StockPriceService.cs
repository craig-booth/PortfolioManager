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
        private Dictionary<string, decimal> _StockQuoteCache;

        internal StockPriceService(IStockQuery stockQuery, IStockPriceDownloader stockPriceDownloader)
        {
            _StockQuery = stockQuery;
            _StockPriceDownloader = stockPriceDownloader;
            _StockQuoteCache = new Dictionary<string, decimal>();
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
            decimal price;

            if (_StockQuoteCache.TryGetValue(stock.ASXCode, out price))
                return price;
            else
                return GetClosingPrice(stock, DateTime.Today);
        }

        public void UpdateCache()
        {
            var stocks = _StockQuery.GetAll(DateTime.Today);

            List<string> asxCodes = new List<string>();
            foreach (var stock in stocks)
            {
                if (stock.ParentId == Guid.Empty)
                    asxCodes.Add(stock.ASXCode);
            }

            var stockQuotes = _StockPriceDownloader.GetMultipleQuotes(asxCodes);
            foreach (var stockQuote in stockQuotes)
            {
                var stock = stocks.FirstOrDefault(x => x.ASXCode == stockQuote.ASXCode);
                if (stock != null)
                {
                    if (_StockQuoteCache.ContainsKey(stock.ASXCode))
                        _StockQuoteCache[stock.ASXCode] = stockQuote.Price;
                    else
                        _StockQuoteCache.Add(stock.ASXCode, stockQuote.Price * 10);

                    if (stock.Type == StockType.StapledSecurity)
                    {
                        var childStocks = stocks.Where(x => x.ParentId == stock.Id);
                        foreach (var childStock in childStocks)
                        {
                            var percentOfPrice = _StockQuery.PercentOfParentCost(stock.Id, childStock.Id, DateTime.Today);

                            if (_StockQuoteCache.ContainsKey(childStock.ASXCode))
                                _StockQuoteCache[childStock.ASXCode] = stockQuote.Price  * percentOfPrice;
                            else
                                _StockQuoteCache.Add(childStock.ASXCode, stockQuote.Price * percentOfPrice);
                        }
                    }
                }
            }
        }
        /*
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
        } */
    /*    
        private bool GetCurrentPrices(IEnumerable<StockPrice> prices)
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
        */
        public decimal GetPrice(Stock stock, DateTime date)
        {
            if (date == DateTime.Today)
                return GetCurrentPrice(stock);
            else
                return GetClosingPrice(stock, date);
        }

    }
}
