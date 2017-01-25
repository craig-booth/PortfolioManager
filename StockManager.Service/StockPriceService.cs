using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockManager.Service.Utils;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace StockManager.Service
{
    public class StockPriceService
    {

        private IStockDatabase _Database;
        private readonly IStockPriceDownloader _StockPriceDownloader;
        private Dictionary<Guid, decimal> _StockQuoteCache;

        public StockPriceService(IStockDatabase database, IStockPriceDownloader stockPriceDownloader)
        {
            _Database = database;

            _StockPriceDownloader = stockPriceDownloader;
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

        public void ImportStockPrices(string fileName)
        {
            var importer = new StockEasyPriceImporter(fileName);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                importer.ImportToDatabase(_Database.StockQuery, unitOfWork);

                unitOfWork.Save();
            }
        }

        internal void UpdateCache()
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
    }
}
 