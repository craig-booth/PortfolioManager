using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Memory.Stocks
{
    public class MemoryStockRepository : IStockRepository 
    {
        private MemoryStockDatabase _Database;

        protected internal MemoryStockRepository(MemoryStockDatabase database)
        {
            _Database = database;
        }

        public Stock Get(Guid id)
        {
            return _Database._Stocks.Find(x => x.Id == id);
        }

        public Stock Get(Guid id, DateTime atDate)
        {
            return _Database._Stocks.Find(x => (x.Id == id) && (x.FromDate <= atDate) && (x.ToDate >= atDate));
        }

        public void Add(Stock entity) 
        {
            _Database._Stocks.Add(entity);
        }

        public void Update(Stock entity)
        {

        }

        public void Delete(Stock entity)
        {

        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid id, DateTime atDate)
        {
        }
    }
}
