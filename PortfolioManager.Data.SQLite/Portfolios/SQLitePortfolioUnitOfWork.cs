using Microsoft.Data.Sqlite;

using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.SQLite.Portfolios.Transactions;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioUnitOfWork : IPortfolioUnitOfWork
    {
        private SqliteTransaction _Transaction;
        private SQLitePortfolioEntityCreator _EntityCreator;

        protected internal SQLitePortfolioUnitOfWork(SqliteTransaction transaction)
        {
            _Transaction = transaction;

            _EntityCreator = new Portfolios.SQLitePortfolioEntityCreator();
        }

        private SQLiteTransactionRepository _TransactionRepository;
        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (_TransactionRepository == null)
                    _TransactionRepository = new SQLiteTransactionRepository(_Transaction);

                return _TransactionRepository;
            }
        }

        private SQLiteParcelRepository _ParcelRepository;
        public IParcelRepository ParcelRepository
        {
            get
            {
                if (_ParcelRepository == null)
                    _ParcelRepository = new SQLiteParcelRepository(_Transaction, _EntityCreator);

                return _ParcelRepository;
            }
        }

        private SQLiteCGTEventRepository _CGTEventRepository;
        public ICGTEventRepository CGTEventRepository
        {
            get
            {
                if (_CGTEventRepository == null)
                    _CGTEventRepository = new SQLiteCGTEventRepository(_Transaction);

                return _CGTEventRepository;
            }
        }

        private SQLiteAttachmentRepository _AttachmentRepository;
        public IAttachmentRepository AttachmentRepository
        {
            get
            {
                if (_AttachmentRepository == null)
                    _AttachmentRepository = new SQLiteAttachmentRepository(_Transaction, _EntityCreator);

                return _AttachmentRepository;
            }
        }

        private SQLiteCashAccountRepository _CashAccountRepository;
        public ICashAccountRepository CashAccountRepository
        {
            get
            {
                if (_CashAccountRepository == null)
                    _CashAccountRepository = new SQLiteCashAccountRepository(_Transaction);

                return _CashAccountRepository;
            }
        }

        private SQLiteParcelAuditRepository _ParcelAuditRepository;
        public IParcelAuditRepository ParcelAuditRepository
        {
            get
            {
                if (_ParcelAuditRepository == null)
                    _ParcelAuditRepository = new SQLiteParcelAuditRepository(_Transaction);

                return _ParcelAuditRepository;
            }
        }

        private SQLiteStockSettingRepository _StockSettingRepository;
        public IStockSettingRepository StockSettingRepository
        {
            get
            {
                if (_StockSettingRepository == null)
                    _StockSettingRepository = new SQLiteStockSettingRepository(_Transaction);

                return _StockSettingRepository;
            }
        }

        private SQLiteDRPCashBalanceRepository _DRPCashBalanceRepository;
        public IDRPCashBalanceRepository DRPCashBalanceRepository
        {
            get
            {
                if (_DRPCashBalanceRepository == null)
                    _DRPCashBalanceRepository = new SQLiteDRPCashBalanceRepository(_Transaction);

                return _DRPCashBalanceRepository;
            }
        }

        public void Save()
        {
            _Transaction.Commit();
        }

        public void Dispose()
        {
            _Transaction.Dispose();
        }

    }
}
