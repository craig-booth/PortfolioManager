using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Data.Memory.Stocks
{
    class MemoryRelateNTARepository: IRelativeNTARepository 
    {
        private MemoryStockDatabase _Database;

        protected internal MemoryRelateNTARepository(MemoryStockDatabase database)
        {
            _Database = database;
        }

        public RelativeNTA Get(Guid id)
        {
            return null;
        }

        public void Add(RelativeNTA entity) 
        {
            _Database._RelativeNTAs.Add(entity);
        }

        public void Update(RelativeNTA entity)
        {

        }

        public void Delete(RelativeNTA entity)
        {

        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid parent, Guid child, DateTime atDate)
        {

        }

        public void DeleteAll(Guid parent)
        {

        }

        public void DeleteAll(Guid parent, Guid child)
        {

        }
    }
}
