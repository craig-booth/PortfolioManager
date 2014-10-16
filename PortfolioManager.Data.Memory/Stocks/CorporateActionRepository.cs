using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Data.Memory.Stocks
{
    class MemoryCorporateActionRepository: ICorporateActionRepository 
    {
        private MemoryStockDatabase _Database;

        protected internal MemoryCorporateActionRepository(MemoryStockDatabase database)
        {
            _Database = database;
        }

        public ICorporateAction Get(Guid id)
        {
            return _Database._CorporateActions.Find(x => x.Id == id);
        }

        public void Add(ICorporateAction entity) 
        {
            _Database._CorporateActions.Add(entity);
        }

        public void Update(ICorporateAction entity)
        {

        }

        public void Delete(ICorporateAction entity)
        {

        }

        public void Delete(Guid id)
        {

        }

    }
}
