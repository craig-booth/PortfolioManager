using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Service
{
    public class StockPriceService
    {
        private readonly IStockQuery _StockQuery;

        internal StockPriceService(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
        }

        public decimal GetClosingPrice(Stock stock, DateTime date)
        {
            return _StockQuery.GetClosingPrice(stock.Id, date);
        }

    }
}
