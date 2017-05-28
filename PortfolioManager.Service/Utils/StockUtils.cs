using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockManager.Service;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Utils
{
    class StockUtils
    {
        private IStockQuery _StockQuery;
        private StockServiceRepository _StockService;

        internal StockUtils(IStockQuery stockQuery, IStockDatabase stockDatabase)
        {
            _StockQuery = stockQuery;
            _StockService = new StockServiceRepository(stockDatabase);
        }

        public StockItem CreateStockItem(Stock stock)
        {
            return new StockItem(stock);
        }

        public StockItem Get(Guid stock, DateTime date)
        {
            return CreateStockItem(_StockQuery.Get(stock, date));
        }

        public StockItem Get(string asxCode, DateTime date)
        {
            return CreateStockItem(_StockQuery.GetByASXCode(asxCode, date));
        } 

        public decimal GetClosingPrice(Guid stock, DateTime date)
        {
            return _StockQuery.GetClosingPrice(stock, date);
        } 

        public decimal GetCurrentPrice(Guid stock)
        {
            return _StockService.StockPriceService.GetCurrentPrice(_StockQuery.Get(stock, DateTime.Today));
        }

        public decimal GetPrice(Guid stock, DateTime atDate)
        {
            if (atDate == DateTime.Today)
                return GetCurrentPrice(stock);
            else
                return GetClosingPrice(stock, atDate);
        }

        public Dictionary<DateTime, decimal> GetClosingPrices(Guid stock, DateTime fromDate, DateTime toDate)
        {
            return _StockService.StockPriceService.GetClosingPrices(_StockQuery.Get(stock, fromDate), fromDate, toDate);
        } 

        public bool TradingDay(DateTime date)
        {
            return _StockService.StockPriceService.TradingDay(date);
        } 
    }
}
