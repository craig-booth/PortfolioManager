using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Stocks.CorporateActions;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteStockReadOnlyUnitOfWork : IStockReadOnlyUnitOfWork
    {
        private SqliteConnection _Connection;
        private SqliteTransaction _Transaction;
        private SQLiteStockEntityCreator _EntityCreator;

        protected internal SQLiteStockReadOnlyUnitOfWork(string fileName)
        {
            _Connection = new SqliteConnection("Data Source=" + fileName);
            _Connection.Open();

            _Transaction = _Connection.BeginTransaction();

            _EntityCreator = new SQLiteStockEntityCreator();
        }

        private SQLiteStockQuery _StockQuery;
        public IStockQuery StockQuery
        {
            get
            {
                if (_StockQuery == null)
                    _StockQuery = new SQLiteStockQuery(_Transaction);

                return _StockQuery;
            }
        }

        private SQLiteCorporateActionQuery _CorporateActionQuery;
        public ICorporateActionQuery CorporateActionQuery
        {
            get
            {
                if (_CorporateActionQuery == null)
                    _CorporateActionQuery = new SQLiteCorporateActionQuery(_Transaction);

                return _CorporateActionQuery;
            }
        }

        public void Dispose()
        {
            _Transaction.Rollback();
            _Connection.Close();
        }

    }
}
