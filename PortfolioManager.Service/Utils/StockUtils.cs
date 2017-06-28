using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Utils
{
    class StockUtils
    {
        private IStockQuery _StockQuery;

        internal StockUtils(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
        }

        public StockItem Get(Guid stock, DateTime date)
        {
            return new StockItem(_StockQuery.Get(stock, date));
        }

        public StockItem Get(string asxCode, DateTime date)
        {
            return new StockItem(_StockQuery.GetByASXCode(asxCode, date));
        } 

        public decimal GetPrice(Guid stock, DateTime date)
        {
            return _StockQuery.GetPrice(stock, date);
        } 

        public Dictionary<DateTime, decimal> GetPrices(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var closingPrices = new Dictionary<DateTime, decimal>();

            var priceData = _StockQuery.GetPrices(stock, fromDate, toDate);

            var priceDataEnumerator = priceData.GetEnumerator();
            priceDataEnumerator.MoveNext();

            decimal lastPrice = 0.00m;
            foreach (var date in DateUtils.DateRange(fromDate, toDate).Where(x => TradingDay(x)))
            {
                var currentPriceData = priceDataEnumerator.Current;

                while (date > currentPriceData.Key)
                {
                    if (!priceDataEnumerator.MoveNext())
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

        public bool TradingDay(DateTime date)
        {
            return _StockQuery.TradingDay(date);
        }  
    }
}
