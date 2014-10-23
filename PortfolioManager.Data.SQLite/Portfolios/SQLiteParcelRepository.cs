using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteParcelRepository : IParcelRepository
    {

        private SQLitePortfolioDatabase _Database;

        protected internal SQLiteParcelRepository(SQLitePortfolioDatabase database)
        {
            _Database = database;
        }

        public ShareParcel Get(Guid id)
        {
            return null;
        }

        public ShareParcel Get(Guid id, DateTime atDate)
        {
            return null;
        }

        public void Add(ShareParcel entity)
        {
            _Database._Parcels.Add(entity);
        }

        public void Update(ShareParcel entity)
        {

        }

        public void Delete(ShareParcel entity)
        {
            _Database._Parcels.Remove(entity);
        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid id, DateTime atDate)
        {
        }
    }
}
