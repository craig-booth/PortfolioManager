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
        private readonly StockCache _Cache;

        internal StockService(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
            _Cache = new StockCache();
        }

        public Stock Get(Guid id, DateTime date)
        {
            Stock stock;

            if (_Cache.Get(id, date, out stock))
                return stock;
            
            stock = _StockQuery.Get(id, date);
            _Cache.Add(stock);

            return stock;
        }

        public Stock Get(string asxCode, DateTime date)
        {
            Stock stock;


            if (_Cache.Get(asxCode, date, out stock))
                return stock;

            stock = _StockQuery.GetByASXCode(asxCode, date);
            _Cache.Add(stock);

            return stock;
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


    class StockCache
    {
        private readonly Dictionary<Guid, Stock> _StockCacheById;
        private readonly Dictionary<string, Stock> _StockCacheByASXCode;

        public StockCache()
        {
            _StockCacheById = new Dictionary<Guid, Stock>();
            _StockCacheByASXCode = new Dictionary<string, Stock>();
        }

        public bool Get(Guid id, DateTime date, out Stock stock)
        {
            if (_StockCacheById.TryGetValue(id, out stock))
            {
                if ((stock.FromDate <= date) && (stock.ToDate >= date))
                    return true;
                else
                    stock = null;
            }

            return false;
        }

        public bool Get(string asxCode, DateTime date, out Stock stock)
        {
            if (_StockCacheByASXCode.TryGetValue(asxCode, out stock))
            {
                if ((stock.FromDate <= date) && (stock.ToDate >= date))
                    return true;
                else
                    stock = null;
            }

            return false;
        }

        public void Add(Stock stock)
        {
            if ((stock.FromDate <= DateTime.Today) && (stock.ToDate >= DateTime.Today))
            {
                _StockCacheById.Add(stock.Id, stock);
                _StockCacheByASXCode.Add(stock.ASXCode, stock);
            }
        }
    }
}
