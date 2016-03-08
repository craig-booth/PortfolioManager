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

        public Transaction Get(Guid id)
        {
            return null;
        }      
        

        public void Add(Transaction entity)
        {
            _Database._Transactions.Add(entity);
        }

        public void Update(Transaction entity)
        {

        }

        public void Delete(Transaction entity)
        {

        }

        public void Delete(Guid id)
        {

        }

    }
}
