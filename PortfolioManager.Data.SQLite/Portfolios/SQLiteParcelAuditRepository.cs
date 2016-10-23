using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteParcelAuditRepository : IParcelAuditRepository
    {

        private SQLitePortfolioDatabase _Database;

        protected internal SQLiteParcelAuditRepository(SQLitePortfolioDatabase database)
        {
            _Database = database;
        }

        public ShareParcelAudit Get(Guid id)
        {
            return null;
        }

        public ShareParcelAudit Get(Guid id, DateTime atDate)
        {
            return null;
        }

        public void Add(ShareParcelAudit entity)
        {
            _Database._ParcelAudit.Add(entity);
        }

        public void Update(ShareParcelAudit entity)
        {

        }

        public void Delete(ShareParcelAudit entity)
        {
            _Database._ParcelAudit.Remove(entity);
        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid id, DateTime atDate)
        {
        }
    }
}
