using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace StockManager.Service.Utils
{

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
            if (stock.IsEffectiveAt(DateTime.Today))
            {
                _StockCacheById.Add(stock.Id, stock);
                _StockCacheByASXCode.Add(stock.ASXCode, stock);
            }
        }

        public void Remove(Stock stock)
        {
            if (stock.IsEffectiveAt(DateTime.Today))
            {
                _StockCacheById.Remove(stock.Id);
                _StockCacheByASXCode.Remove(stock.ASXCode);
            }
        }
    }
}
