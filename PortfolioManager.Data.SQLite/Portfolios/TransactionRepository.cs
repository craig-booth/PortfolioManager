using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Memory.Portfolios
{
    class MemoryTransactionRepository: ITransactionRepository 
    {

        private MemoryPortfolioDatabase _Database;

        protected internal MemoryTransactionRepository(MemoryPortfolioDatabase database)
        {
            _Database = database;
        }

        public ITransaction Get(Guid id)
        {
            return null;
        }      
        

        public void Add(ITransaction entity)
        {
            _Database._Transactions.Add(entity);
        }

        public void Update(ITransaction entity)
        {

        }

        public void Delete(ITransaction entity)
        {

        }


    }
}
