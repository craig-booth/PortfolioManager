using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using StockManager.Service.Utils;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace StockManager.Service
{
    public class StockPriceService
    {

        private readonly IStockDatabase _Database;
        private readonly IStockPriceDownloader _StockPriceDownloader;
        private readonly IHistoricalPriceDownloader _HistoricalPriceDownloader;
        private readonly ITradingDayDownloader _TradingDayDownloader;

        public StockPriceService(IStockDatabase database, IStockPriceDownloader stockPriceDownloader, IHistoricalPriceDownloader historicalPriceDownloader, ITradingDayDownloader tradingDayDownloader)
        {
            _Database = database;

            _StockPriceDownloader = stockPriceDownloader;
            _HistoricalPriceDownloader = historicalPriceDownloader;
            _TradingDayDownloader = tradingDayDownloader;
        }

        public decimal GetPrice(Stock stock, DateTime atDate)
        {
            return _Database.StockQuery.GetPrice(stock.Id, atDate);
        } 

        public decimal GetCurrentPrice(Stock stock)
        {
            return GetPrice(stock, DateTime.Today);
        } 

        public void AddPrice(Stock stock, DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Add(stock.Id, atDate, price, false);
                unitOfWork.Save();
            }
        }

        public void ChangePrice(Stock stock, DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Update(stock.Id, atDate, price, false);
                unitOfWork.Save();
            }
        }

        public Dictionary<DateTime, decimal> GetPrices(Stock stock, DateTime fromDate, DateTime toDate)
        {
            var closingPrices = new Dictionary<DateTime, decimal>();

            var priceData = _Database.StockQuery.GetPrices(stock.Id, fromDate, toDate);

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
            return (date.WeekDay() && ! _Database.StockQuery.TradingDay(date));
        }

        internal async Task DownloadCurrentPrices()
        {
            var stocks = _Database.StockQuery.GetAll(DateTime.Today);

            List<string> asxCodes = new List<string>();
            foreach (var stock in stocks)
            {
                if (stock.ParentId == Guid.Empty)
                    asxCodes.Add(stock.ASXCode);
            }

            var stockQuotes = await _StockPriceDownloader.GetMultipleQuotes(asxCodes);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (var stockQuote in stockQuotes)
                {
                    var stock = stocks.FirstOrDefault(x => x.ASXCode == stockQuote.ASXCode);
                    if (stock != null)
                    {
                        if (unitOfWork.StockPriceRepository.Exists(stock.Id, DateTime.Today))
                        {
                            unitOfWork.StockPriceRepository.Add(stock.Id, DateTime.Today, stockQuote.Price, true);
                        }
                        else
                        {
                            unitOfWork.StockPriceRepository.Update(stock.Id, DateTime.Today, stockQuote.Price, true);
                        }                           

                        if (stock.Type == StockType.StapledSecurity)
                        {
                            var childStocks = stocks.Where(x => x.ParentId == stock.Id);
                            foreach (var childStock in childStocks)
                            {
                                var percentOfPrice = _Database.StockQuery.PercentOfParentCost(stock.Id, childStock.Id, DateTime.Today);

                                if (unitOfWork.StockPriceRepository.Exists(stock.Id, DateTime.Today))
                                {
                                    unitOfWork.StockPriceRepository.Add(childStock.Id, DateTime.Today, stockQuote.Price * percentOfPrice, true);
                                }
                                else
                                {
                                    unitOfWork.StockPriceRepository.Update(childStock.Id, DateTime.Today, stockQuote.Price * percentOfPrice, true);
                                }
                            }
                        }
                    }
                }

                unitOfWork.Save();
            }
        }

        internal async Task DownloadHistoricalPrices()
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
                            var data = await _HistoricalPriceDownloader.GetHistoricalPriceData(stock.ASXCode, latestDate.AddDays(1), lastExpectedDate);
                            UpdateStockPrices(unitOfWork, data);
                        }
                    }
                }

                unitOfWork.Save();
            }
        }

        internal async Task DownloadNonTradingDays(int fromYear)
        {
            for (var year = fromYear; year <= DateTime.Today.Year; year++)
            {
                var nonTradingDays = await _TradingDayDownloader.NonTradingDays(year);

                using (var unitOfWork = _Database.CreateUnitOfWork())
                {
                    foreach (var nonTradingDay in nonTradingDays)
                    {
                        unitOfWork.NonTradingDayRepository.Add(nonTradingDay);
                    }
                }                    
            }
        }

        private void UpdateStockPrices(IStockUnitOfWork unitOfWork, IEnumerable<StockQuote> stockPrices)
        {
            Stock stock = null;

            foreach (var stockPrice in stockPrices)
            {
                if (_Database.StockQuery.TryGetByASXCode(stockPrice.ASXCode, stockPrice.Date, out stock))
                {
                    if (unitOfWork.StockPriceRepository.Exists(stock.Id, stockPrice.Date))
                        unitOfWork.StockPriceRepository.Update(stock.Id, stockPrice.Date, stockPrice.Price, false);
                    else
                        unitOfWork.StockPriceRepository.Add(stock.Id, stockPrice.Date, stockPrice.Price, false);
                }
            }
        } 
    }
}
 