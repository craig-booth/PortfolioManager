using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockManager.Service;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service
{
    public class StockService
    {
        private StockServiceRepository _StockServiceRepository;

        internal StockService(StockServiceRepository stockServiceRepository)
        {
            _StockServiceRepository = stockServiceRepository;
        }

        public Stock Get(Guid id, DateTime date)
        {
            return _StockServiceRepository.StockService.GetStock(id, date);
        }

        public Stock Get(string asxCode, DateTime date)
        {
            return _StockServiceRepository.StockService.GetStock(asxCode, date);
        }

        public IReadOnlyCollection<Stock> GetAll(DateTime atDate)
        {
            return _StockServiceRepository.StockService.GetStocks(atDate);
        }

        public IReadOnlyCollection<Stock> GetChildStocks(Stock stock, DateTime date)
        {
            return _StockServiceRepository.StockService.GetChildStocks(stock);
        }

        public decimal PercentageOfParentCostBase(Stock stock, DateTime atDate)
        {
            return _StockServiceRepository.StockService.PercentageOfParentCostBase(stock, atDate);
        }

        public decimal GetClosingPrice(Stock stock, DateTime atDate)
        {
            return _StockServiceRepository.StockPriceService.GetClosingPrice(stock, atDate);
        }

        public decimal GetCurrentPrice(Stock stock)
        {
            return _StockServiceRepository.StockPriceService.GetCurrentPrice(stock);
        }

        public decimal GetPrice(Stock stock, DateTime atDate)
        {
            if (atDate == DateTime.Today)
                return GetCurrentPrice(stock);
            else
                return GetClosingPrice(stock, atDate);
        }

        public Dictionary<DateTime, decimal> GetClosingPrices(Stock stock, DateTime fromDate, DateTime toDate)
        {
            return _StockServiceRepository.StockPriceService.GetClosingPrices(stock, fromDate, toDate);
        }

    }


}
