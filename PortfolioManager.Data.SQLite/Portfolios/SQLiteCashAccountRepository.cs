using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteCashAccountRepository : ICashAccountRepository
    {
        private SQLitePortfolioDatabase _Database;

        protected internal SQLiteCashAccountRepository(SQLitePortfolioDatabase database)
        {
            _Database = database;
        }

        public CashAccountTransaction Get(Guid id)
        {
            return null;
        }

        public CashAccountTransaction Get(Guid id, DateTime atDate)
        {
            return null;
        }

        public void Add(CashAccountTransaction entity)
        {
            _Database._CashAccountTransactions.Add(entity);
        }

        public void Update(CashAccountTransaction entity)
        {

        }

        public void Delete(CashAccountTransaction entity)
        {
            _Database._CashAccountTransactions.Remove(entity);
        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid id, DateTime atDate)
        {
        }
    }
}
