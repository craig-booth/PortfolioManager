using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLitePortfolioReadOnlyUnitOfWork : IPortfolioReadOnlyUnitOfWork
    {
        private SqliteConnection _Connection;
        private SqliteTransaction _Transaction;
        private SQLitePortfolioEntityCreator _EntityCreator;

        protected internal SQLitePortfolioReadOnlyUnitOfWork(string fileName)
        {
            _Connection = new SqliteConnection("Data Source=" + fileName);
            _Connection.Open();

            _Transaction = _Connection.BeginTransaction();

            _EntityCreator = new SQLitePortfolioEntityCreator();
        }

        private SQLitePortfolioQuery _PortfolioQuery;
        public IPortfolioQuery PortfolioQuery
        {
            get
            {
                if (_PortfolioQuery == null)
                    _PortfolioQuery = new SQLitePortfolioQuery(_Transaction);

                return _PortfolioQuery;
            }

        }

        public void Dispose()
        {
            _Transaction.Rollback();
            _Connection.Close();
        }
    }
}
