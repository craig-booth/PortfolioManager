using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.Memory.Portfolios
{
    public class MemoryPortfolioRepository: IPortfolioRepository 
    {

        private MemoryPortfolioDatabase _Database;

        protected internal MemoryPortfolioRepository(MemoryPortfolioDatabase database)
        {
            _Database = database;
        }

        public Portfolio Get(Guid id)
        {
            return null;
        }      
        

        public void Add(Portfolio entity)
        {
            _Database._Portfolios.Add(entity);
        }

        public void Update(Portfolio entity)
        {

        }

        public void Delete(Portfolio entity)
        {
            _Database._Portfolios.Remove(entity);
        }
    }
}
