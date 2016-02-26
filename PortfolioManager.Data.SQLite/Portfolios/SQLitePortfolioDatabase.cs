using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioDatabase: SQLiteDatabase, IPortfolioDatabase
    {
        protected override int RepositoryVersion
        {
            get { return 3; }
        }

        /* TODO: Priority Low, move this to the database */
        internal List<ShareParcel> _Parcels { get; private set; }
        internal List<CGTEvent> _CGTEvents { get; private set; }
        internal List<IncomeReceived> _IncomeReceived { get; private set; }

        public IPortfolioQuery PortfolioQuery { get; private set; }

        public SQLitePortfolioDatabase(string fileName) : base(fileName)
        {
            PortfolioQuery = new SQLitePortfolioQuery(this);

            _Parcels = new List<ShareParcel>();
            _CGTEvents = new List<CGTEvent>();
            _IncomeReceived = new List<IncomeReceived>();
        }

        protected override SQLiteDatabaseUpgrade GetUpgrade(int forVersion)
        {
            if (forVersion == 0)
                return new SQLiteSimpleDatabaseUpgrade(1, "Upgrade\\PortfolioDatabaseUpgradeToVersion1.sql");
            else if (forVersion == 1)
                return new SQLiteSimpleDatabaseUpgrade(2, "Upgrade\\PortfolioDatabaseUpgradeToVersion2.sql");
            else if (forVersion == 2)
                return new SQLiteSimpleDatabaseUpgrade(3, "Upgrade\\PortfolioDatabaseUpgradeToVersion3.sql");
            else
                throw new NotSupportedException();
        }

        public IPortfolioUnitOfWork CreateUnitOfWork()
        {
            return new SQLitePortfolioUnitOfWork(this);
        }
     

        protected override void CreateDatabaseTables()
        {
            ExecuteScript("Portfolio Database.sql");
        }
    }
}
