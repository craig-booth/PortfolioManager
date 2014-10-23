using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioUnitOfWork : IPortfolioUnitOfWork
    {
        private SQLitePortfolioDatabase _Database;
        private SQLitePortfolioRepository _PortfolioRepository;
        private SQLiteParcelRepository _ParcelRepository;
        private SQLiteTransactionRepository _TransactionRepository;
        private SQLiteCGTEventRepository _CGTEventRepository;

        public IPortfolioRepository PortfolioRepository
        {
            get
            {
                if (_PortfolioRepository == null)
                    _PortfolioRepository = new SQLitePortfolioRepository(_Database);

                return _PortfolioRepository;
            }
        }

        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (_TransactionRepository == null)
                    _TransactionRepository = new SQLiteTransactionRepository(_Database);

                return _TransactionRepository;
            }
        }

        public IParcelRepository ParcelRepository
        {
            get
            {
                if (_ParcelRepository == null)
                    _ParcelRepository = new SQLiteParcelRepository(_Database);

                return _ParcelRepository;
            }
        }

        public ICGTEventRepository CGTEventRepository
        {
            get
            {
                if (_CGTEventRepository == null)
                    _CGTEventRepository = new SQLiteCGTEventRepository(_Database);

                return _CGTEventRepository;
            }
        }

        protected internal SQLitePortfolioUnitOfWork(SQLitePortfolioDatabase database)
        {
            _Database = database;
            _Database._Transaction.BeginTransaction();
        }

        public void Save()
        {
            _Database._Transaction.SaveOnEnd = true;
        }

        public void Dispose()
        {
            _Database._Transaction.EndTransaction();
        }

    }
}
