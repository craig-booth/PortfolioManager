using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Utils
{
    static class StockUtils
    {

        public static StockItem ToStockItem(this Domain.Stocks.Stock stock, DateTime date)
        {
            var stockProperties = stock.Properties[date];
            return new StockItem(stock.Id, stockProperties.ASXCode, stockProperties.Name);
        }

        public static StockItem Get(Guid stock, DateTime date, IStockQuery stockQuery)
        {
            return new StockItem(stockQuery.Get(stock, date));
        }

        public static StockItem Get(string asxCode, DateTime date, IStockQuery stockQuery)
        {
            return new StockItem(stockQuery.GetByASXCode(asxCode, date));
        }

        public static decimal GetPrice(Guid stock, DateTime date, IStockQuery stockQuery)
        {
            return stockQuery.GetPrice(stock, date);
        }

        public static Dictionary<DateTime, decimal> GetPrices(Guid stock, DateTime fromDate, DateTime toDate, IStockQuery stockQuery)
        {
            var closingPrices = new Dictionary<DateTime, decimal>();

            var priceData = stockQuery.GetPrices(stock, fromDate, toDate);

            var priceDataEnumerator = priceData.GetEnumerator();
            priceDataEnumerator.MoveNext();

            decimal lastPrice = 0.00m;
            foreach (var date in DateUtils.DateRange(fromDate, toDate).Where(x => TradingDay(x, stockQuery)))
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

        public static bool TradingDay(DateTime date, IStockQuery stockQuery)
        {
            return stockQuery.TradingDay(date);
        }  
    }
}
