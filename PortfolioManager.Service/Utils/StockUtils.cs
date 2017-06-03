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
        private ILiveStockPriceQuery _LivePriceQuery;

        internal StockUtils(IStockQuery stockQuery, ILiveStockPriceQuery livePriceQuery)
        {
            _StockQuery = stockQuery;
            _LivePriceQuery = livePriceQuery;
        }


        public StockItem Get(Guid stock, DateTime date)
        {
            return new StockItem(_StockQuery.Get(stock, date));
        }

        public StockItem Get(string asxCode, DateTime date)
        {
            return new StockItem(_StockQuery.GetByASXCode(asxCode, date));
        } 

        public decimal GetClosingPrice(Guid stock, DateTime date)
        {
            return _StockQuery.GetClosingPrice(stock, date);
        } 

            public decimal GetCurrentPrice(Guid stock)
            {          
                return _LivePriceQuery.GetPrice(stock);
            }

            public decimal GetPrice(Guid stock, DateTime atDate)
            {
                if (atDate.Date == DateTime.Today)
                    return GetCurrentPrice(stock);
                else
                    return GetClosingPrice(stock, atDate);
            }

        public Dictionary<DateTime, decimal> GetClosingPrices(Guid stock, DateTime fromDate, DateTime toDate)
        {
            return new Dictionary<DateTime, decimal>();
            //return _StockService.StockPriceService.GetClosingPrices(_StockQuery.Get(stock, fromDate), fromDate, toDate);
        }  

        public bool TradingDay(DateTime date)
        {
            return true;
            //return _StockService.StockPriceService.TradingDay(date);
        }  
    }
}
