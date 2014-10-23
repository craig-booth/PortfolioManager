using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioRepository : IPortfolioRepository
    {
        private SQLitePortfolioDatabase _Database;

        protected internal SQLitePortfolioRepository(SQLitePortfolioDatabase database)
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

        public void Delete(Guid id)
        {

        }

    }
}
