using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Memory.Portfolios
{
    class MemoryCGTEventRepository : ICGTEventRepository 
    {

        private MemoryPortfolioDatabase _Database;

        protected internal MemoryCGTEventRepository(MemoryPortfolioDatabase database)
        {
            _Database = database;
        }

        public CGTEvent Get(Guid id)
        {
            return null;
        }


        public void Add(CGTEvent entity)
        {
            _Database._CGTEvents.Add(entity);
        }

        public void Update(CGTEvent entity)
        {

        }

        public void Delete(CGTEvent entity)
        {

        }

        public void Delete(Guid id)
        {

        }

    }
}
