using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Data.SQLite.Upgrade;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioDatabase : SQLiteDatabase, IPortfolioDatabase
    {
        protected override int RepositoryVersion
        {
            get { return 8; }
        }

        internal List<ShareParcel> _Parcels { get; private set; }
        internal List<CGTEvent> _CGTEvents { get; private set; }
        internal List<IncomeReceived> _IncomeReceived { get; private set; }
        internal List<CashAccountTransaction> _CashAccountTransactions { get; private set; }
        internal List<ShareParcelAudit> _ParcelAudit { get; private set; }

        public IPortfolioQuery PortfolioQuery { get; private set; }

        public SQLitePortfolioDatabase(string fileName) : base(fileName)
        {
            PortfolioQuery = new SQLitePortfolioQuery(this);

            _Parcels = new List<ShareParcel>();
            _CGTEvents = new List<CGTEvent>();
            _IncomeReceived = new List<IncomeReceived>();
            _CashAccountTransactions = new List<CashAccountTransaction>();
            _ParcelAudit = new List<ShareParcelAudit>();
        }

        protected override SQLiteDatabaseUpgrade GetUpgrade(int forVersion)
        {
            if (forVersion == 0)
                return new SQLiteSimpleDatabaseUpgrade(1, "Upgrade\\PortfolioDatabaseUpgradeToVersion1.sql");
            else if (forVersion == 1)
                return new SQLiteSimpleDatabaseUpgrade(2, "Upgrade\\PortfolioDatabaseUpgradeToVersion2.sql");
            else if (forVersion == 2)
                return new SQLiteSimpleDatabaseUpgrade(3, "Upgrade\\PortfolioDatabaseUpgradeToVersion3.sql");
            else if (forVersion == 3)
                return new SQLiteSimpleDatabaseUpgrade(4, "Upgrade\\PortfolioDatabaseUpgradeToVersion4.sql");
            else if (forVersion == 4)
                return new SQLiteSimpleDatabaseUpgrade(5, "Upgrade\\PortfolioDatabaseUpgradeToVersion5.sql");
            else if (forVersion == 5)
                return new SQLiteSimpleDatabaseUpgrade(6, "Upgrade\\PortfolioDatabaseUpgradeToVersion6.sql");
            else if (forVersion == 6)
                return new SQLiteSimpleDatabaseUpgrade(7, "Upgrade\\PortfolioDatabaseUpgradeToVersion7.sql");
            else if (forVersion == 7)
                return new SQLiteSimpleDatabaseUpgrade(8, "Upgrade\\PortfolioDatabaseUpgradeToVersion8.sql");
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
