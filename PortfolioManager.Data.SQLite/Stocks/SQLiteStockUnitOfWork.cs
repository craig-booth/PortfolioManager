using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Stocks.CorporateActions;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteStockUnitOfWork : IStockUnitOfWork
    {
        private SqliteConnection _Connection;
        private SqliteTransaction _Transaction;
        private SQLiteStockEntityCreator _EntityCreator;

        protected internal SQLiteStockUnitOfWork(string fileName)
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

        SQLiteStockRepository _StockRepository;
        public IStockRepository StockRepository
        {
            get
            {
                if (_StockRepository == null)
                    _StockRepository = new SQLiteStockRepository(_Transaction, _EntityCreator);

                return _StockRepository;
            }
        }

        SQLiteCorporateActionRepository _CorporateActionRepository;
        public ICorporateActionRepository CorporateActionRepository
        {
            get
            {
                if (_CorporateActionRepository == null)
                    _CorporateActionRepository = new SQLiteCorporateActionRepository(_Transaction, _EntityCreator);

                return _CorporateActionRepository;
            }
        }

        SQLiteRelativeNTARepository _RelativeNTARepository;
        public IRelativeNTARepository RelativeNTARepository
        {
            get
            {
                if (_RelativeNTARepository == null)
                    _RelativeNTARepository = new SQLiteRelativeNTARepository(_Transaction, _EntityCreator);

                return _RelativeNTARepository;
            }
        }

        SQLiteStockPriceRepository _StockPriceRepository;
        public IStockPriceRepository StockPriceRepository
        {
            get
            {
                if (_StockPriceRepository == null)
                    _StockPriceRepository = new SQLiteStockPriceRepository(_Transaction);

                return _StockPriceRepository;
            }
        }

        SQLiteNonTradingDayRepository _NonTradingDayRepository;
        public INonTradingDayRepository NonTradingDayRepository
        {
            get
            {
                if (_NonTradingDayRepository == null)
                    _NonTradingDayRepository = new SQLiteNonTradingDayRepository(_Transaction);

                return _NonTradingDayRepository;
            }
        }

        public void Save()
        {
            _Transaction.Commit();
        }

        public void Dispose()
        {
            _Transaction.Dispose();
            _Connection.Close();
        }
    }


}
