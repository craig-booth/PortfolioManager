using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockManager.Service.Utils;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

namespace StockManager.Service
{
    public class StockPriceService
    {

        private readonly IStockDatabase _Database;
        private readonly IStockPriceDownloader _StockPriceDownloader;
        private readonly IHistoricalPriceDownloader _HistoricalPriceDownloader;
        private readonly ITradingDayDownloader _TradingDayDownloader;

        private HashSet<DateTime> _NonTradingDays;

        private Dictionary<Guid, decimal> _StockQuoteCache;

        public StockPriceService(IStockDatabase database, IStockPriceDownloader stockPriceDownloader, IHistoricalPriceDownloader historicalPriceDownloader, ITradingDayDownloader tradingDayDownloader)
        {
            _Database = database;

            _NonTradingDays = new HashSet<DateTime>();

            _StockPriceDownloader = stockPriceDownloader;
            _HistoricalPriceDownloader = historicalPriceDownloader;
            _TradingDayDownloader = tradingDayDownloader;

            _StockQuoteCache = new Dictionary<Guid, decimal>();
        }

        public decimal GetClosingPrice(Stock stock, DateTime atDate)
        {
            return _Database.StockQuery.GetClosingPrice(stock.Id, atDate);
        } 

        public decimal GetCurrentPrice(Stock stock)
        {
            decimal price;

            if (_StockQuoteCache.TryGetValue(stock.Id, out price))
                return price;
            else
                return GetClosingPrice(stock, DateTime.Today);
        } 

        public void AddPrice(Stock stock, DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Add(stock.Id, atDate, price);
                unitOfWork.Save();
            }
        }

        public void ChangePrice(Stock stock, DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Update(stock.Id, atDate, price);
                unitOfWork.Save();
            }
        }

        public Dictionary<DateTime, decimal> GetClosingPrices(Stock stock, DateTime fromDate, DateTime toDate)
        {
            var closingPrices = new Dictionary<DateTime, decimal>();

            var priceData = _Database.StockQuery.GetClosingPrices(stock.Id, fromDate, toDate);

            // Add current price
            if (toDate >= DateTime.Today)
            {
                var currentPrice = GetCurrentPrice(stock);

                if (priceData.ContainsKey(DateTime.Today))
                    priceData[DateTime.Today] = currentPrice;
                else
                    priceData.Add(DateTime.Today, currentPrice);
            }

            var priceDataEnumerator = priceData.GetEnumerator();
            priceDataEnumerator.MoveNext();

            decimal lastPrice = 0.00m;
            foreach (var date in DateUtils.DateRange(fromDate, toDate).Where(x => TradingDay(x)))
            {
                var currentPriceData = priceDataEnumerator.Current;

                while (date > currentPriceData.Key)
                {
                    if (! priceDataEnumerator.MoveNext())
                        break;
                    currentPriceData = priceDataEnumerator.Current;
                }

                if (date == currentPriceData.Key)
                {
                    closingPrices.Add(date, currentPriceData.Value);
                    lastPrice = currentPriceData.Value;

                    priceDataEnumerator.MoveNext();
                }
                else
                {
                    closingPrices.Add(date, lastPrice);
                }
               
            }

            return closingPrices;
        }

        public void ImportStockPrices(string fileName)
        {
            var importer = new StockEasyPriceImporter(fileName);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                importer.ImportToDatabase(_Database.StockQuery, unitOfWork);

                unitOfWork.Save();
            }
        }

        public bool TradingDay(DateTime date)
        {
            return (date.WeekDay() && !_NonTradingDays.Contains(date));
        }

        internal void DownloadCurrentPrices()
        {
            var stocks = _Database.StockQuery.GetAll(DateTime.Today);

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
                    if (_StockQuoteCache.ContainsKey(stock.Id))
                        _StockQuoteCache[stock.Id] = stockQuote.Price;
                    else
                        _StockQuoteCache.Add(stock.Id, stockQuote.Price);

                    if (stock.Type == StockType.StapledSecurity)
                    {
                        var childStocks = stocks.Where(x => x.ParentId == stock.Id);
                        foreach (var childStock in childStocks)
                        {
                            var percentOfPrice = _Database.StockQuery.PercentOfParentCost(stock.Id, childStock.Id, DateTime.Today);

                            if (_StockQuoteCache.ContainsKey(childStock.Id))
                                _StockQuoteCache[childStock.Id] = stockQuote.Price * percentOfPrice;
                            else
                                _StockQuoteCache.Add(childStock.Id, stockQuote.Price * percentOfPrice);
                        }
                    }
                }
            }
        }

        internal void DownloadHistoricalPrices()
        {
            DateTime lastExpectedDate;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                lastExpectedDate = DateTime.Today.AddDays(-3);
            else if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                lastExpectedDate = DateTime.Today.AddDays(-2);
            else
                lastExpectedDate = DateTime.Today.AddDays(-1);

            var stocks = _Database.StockQuery.GetAll();

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (var stock in stocks)
                {
                    if (stock.ParentId == Guid.Empty)
                    {
                        var latestDate = _Database.StockQuery.GetLatestClosingPrice(stock.Id);

                        if (latestDate < lastExpectedDate)
                        {
                            var data = _HistoricalPriceDownloader.GetHistoricalPriceData(stock.ASXCode, latestDate.AddDays(1), lastExpectedDate);
                            UpdateStockPrices(unitOfWork, data);
                        }
                    }
                }

                unitOfWork.Save();
            }
        }

        internal void DownloadNonTradingDays(int fromYear)
        {
            for (var year = fromYear; year <= DateTime.Today.Year; year++)
            {
                foreach (var nonTradingDay in _TradingDayDownloader.NonTradingDays(year))
                    _NonTradingDays.Add(nonTradingDay);
            }
        }

        private void UpdateStockPrices(IStockUnitOfWork unitOfWork, IEnumerable<StockQuote> stockPrices)
        {
             Stock stock = null;

            foreach (var stockPrice in stockPrices)
            {
                if (_Database.StockQuery.TryGetByASXCode(stockPrice.ASXCode, stockPrice.Date, out stock))
                {
                    decimal price;
                    if (_Database.StockQuery.TryGetClosingPrice(stock.Id, stockPrice.Date, out price, true))
                        unitOfWork.StockPriceRepository.Update(stock.Id, stockPrice.Date, stockPrice.Price);
                    else
                        unitOfWork.StockPriceRepository.Add(stock.Id, stockPrice.Date, stockPrice.Price);
                }
            }
        } 
    }
}
 