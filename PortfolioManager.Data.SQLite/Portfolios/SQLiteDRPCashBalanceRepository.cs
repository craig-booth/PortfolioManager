using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteDRPCashBalanceRepository : IDRPCashBalanceRepository
    {
        private SQLitePortfolioDatabase _Database;

        protected internal SQLiteDRPCashBalanceRepository(SQLitePortfolioDatabase database)
        {
            _Database = database;
        }

        public DRPCashBalance Get(Guid id)
        {
            return Get(id, DateTime.Today);
        }

        public DRPCashBalance Get(Guid id, DateTime atDate)
        {
            var drpBalanceQuery = from drpBalance in _Database._DRPCashBalances
                                  where (drpBalance.Id == id) && ((atDate >= drpBalance.FromDate && atDate <= drpBalance.ToDate))
                                  select drpBalance;

            return drpBalanceQuery.FirstOrDefault();
        }

        public void Add(DRPCashBalance entity)
        {
            _Database._DRPCashBalances.Add(entity);
        }

        public void Update(DRPCashBalance entity)
        {

        }

        public void Delete(DRPCashBalance entity)
        {
            _Database._DRPCashBalances.Remove(entity);
        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid id, DateTime atDate)
        {
        }
    }
}
