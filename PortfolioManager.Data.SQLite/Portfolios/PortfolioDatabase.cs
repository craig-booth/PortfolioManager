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
        public IPortfolioUnitOfWork CreateUnitOfWork()
        {
            return null; //new SQLitePortfolioUnitOfWork(this);
        }

        public IPortfolioQuery PortfolioQuery {get; private set;}

        public SQLitePortfolioDatabase(string connectionString) : base(connectionString)
        {
            // PortfolioQuery = new SQLitePortfolioQuery(this);
        }

        protected override void CreateDatabaseTables()
        {
            CreateDatabaseTables("Portfolio Database.sql");
        }
    }
}
