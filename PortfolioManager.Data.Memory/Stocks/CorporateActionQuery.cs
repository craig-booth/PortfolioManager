using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Data.Memory.Stocks
{
    class MemoryCorporateActionQuery: ICorporateActionQuery
    {
        private MemoryStockDatabase _Database;

        protected internal MemoryCorporateActionQuery(MemoryStockDatabase database)
        {
            _Database = database;
        }

        public CorporateAction Get(Guid id)
        {
            return _Database._CorporateActions.Find(x => x.Id == id);
        }

        public IReadOnlyCollection<CorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var corporateActions = from action in _Database._CorporateActions 
                                   where action.Stock == stock && action.ActionDate >= fromDate && action.ActionDate <= toDate
                                   select action;

            return corporateActions.ToList().AsReadOnly();
        }
    }
}
