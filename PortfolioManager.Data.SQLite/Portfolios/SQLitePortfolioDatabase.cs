using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioDatabase: SQLiteDatabase, IPortfolioDatabase
    {
        /* TODO: Priority Low, move this to the database */
        internal List<Portfolio> _Portfolios { get; private set; }
        internal List<ShareParcel> _Parcels { get; private set; }
        internal List<CGTEvent> _CGTEvents { get; private set; }
        internal List<IncomeReceived> _IncomeReceived { get; private set; }

        public IPortfolioUnitOfWork CreateUnitOfWork()
        {
            return new SQLitePortfolioUnitOfWork(this);
        }

        public IPortfolioQuery PortfolioQuery {get; private set;}

        public SQLitePortfolioDatabase(string connectionString) : base(connectionString)
        {
            PortfolioQuery = new SQLitePortfolioQuery(this);

            _Portfolios = new List<Portfolio>();
            _Parcels = new List<ShareParcel>();
            _CGTEvents = new List<CGTEvent>();
            _IncomeReceived = new List<IncomeReceived>();
        }

        protected override void CreateDatabaseTables()
        {
            CreateDatabaseTables("Portfolio Database.sql");
        }
    }
}
