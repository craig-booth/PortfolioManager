using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class StockService
    {
        private readonly IStockQuery _StockQuery;

        public StockService(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
        }

        public Stock Get(Guid id, DateTime date)
        {
            return _StockQuery.Get(id, date);
        }

        public Stock Get(string asxCode, DateTime date)
        {
            return _StockQuery.GetByASXCode(asxCode, date);
        }

        public IReadOnlyCollection<Stock> GetAll(DateTime atDate)
        {
            return _StockQuery.GetAll(atDate);
        }

        public IReadOnlyCollection<Stock> GetChildStocks(Stock stock, DateTime date)
        {
            return _StockQuery.GetChildStocks(stock.Id, date);
        }

    }
}
