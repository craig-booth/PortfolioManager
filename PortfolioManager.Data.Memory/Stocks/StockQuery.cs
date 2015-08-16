using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Memory.Stocks
{
    class MemoryStockQuery: IStockQuery 
    {
        private MemoryStockDatabase _Database;

        protected internal MemoryStockQuery(MemoryStockDatabase database)
        {
            _Database = database;
        }

        public Stock Get(Guid id, DateTime atDate)
        {
            return _Database._Stocks.Find(x => (x.Id == id) && (x.FromDate <= atDate) && (x.ToDate >= atDate));
        }

        public IReadOnlyCollection<Stock> GetAll()
        {
            return null;
        }

        public IReadOnlyCollection<Stock> GetAll(DateTime atDate)
        {
            return null;
        }

        public Stock GetByASXCode(string asxCode)
        {
            return GetByASXCode(asxCode, DateTime.Today);
        }

        public Stock GetByASXCode(string asxCode, DateTime atDate)
        {
            Stock stock;
            try
            {
                stock = _Database._Stocks.First(x => (x.ASXCode == asxCode) && (x.FromDate >= atDate) && (x.ToDate <= atDate));
            }
            catch
            {
                throw new RecordNotFoundException(String.Format("ASX Code {0} did not exist at {1}", asxCode, atDate));
            }

            return stock;
        }

        public IReadOnlyCollection<Stock> GetChildStocks(Guid parent, DateTime atDate)
        {
            var childStocks = from childStock in _Database._Stocks
                              where childStock.ParentId == parent
                              select childStock;

            return childStocks.ToList().AsReadOnly();
        }

        public RelativeNTA GetRelativeNTA(Guid parent, Guid child, DateTime toDate)
        {
            return null;
        }

        public IReadOnlyCollection<RelativeNTA> GetRelativeNTAs(Guid parent, Guid child)
        {
            return null;
        }

        public decimal PercentOfParentCost(Guid parent, Guid child, DateTime atDate)
        {
            var percentQuery = from relativeNTA in _Database._RelativeNTAs
                               where (relativeNTA.Date <= atDate) && (relativeNTA.Parent == parent) && (relativeNTA.Child == child)
                               orderby relativeNTA.Date descending
                               select relativeNTA;

            return percentQuery.First().Percentage;
        }

        public string GetASXCode(Guid id, DateTime atDate)
        {
            return Get(id, atDate).ASXCode;
        }
    }
}
