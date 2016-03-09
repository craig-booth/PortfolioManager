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
        private SQLiteParcelRepository _ParcelRepository;
        private SQLiteTransactionRepository _TransactionRepository;
        private SQLiteCGTEventRepository _CGTEventRepository;
        private SQLiteAttachmentRepository _AttachmentRepository;

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

        public IAttachmentRepository AttachmentRepository
        {
            get
            {
                if (_AttachmentRepository == null)
                    _AttachmentRepository = new SQLiteAttachmentRepository(_Database);

                return _AttachmentRepository;
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
