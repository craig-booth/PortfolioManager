using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    public class SQLitePortfolioUnitOfWork : IPortfolioUnitOfWork
    {
        private SQLitePortfolioDatabase _Database;
        private SQLitePortfolioEntityCreator _EntityCreator;
        private SQLiteParcelRepository _ParcelRepository;
        private SQLiteTransactionRepository _TransactionRepository;
        private SQLiteCGTEventRepository _CGTEventRepository;
        private SQLiteAttachmentRepository _AttachmentRepository;
        private SQLiteCashAccountRepository _CashAccountRepository;
        private SQLiteParcelAuditRepository _ParcelAuditRepository;
        private SQLiteStockSettingRepository _StockSettingRepository;
        private SQLiteDRPCashBalanceRepository _DRPCashBalanceRepository;

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
                    _ParcelRepository = new SQLiteParcelRepository(_Database, _EntityCreator);

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
                    _AttachmentRepository = new SQLiteAttachmentRepository(_Database, _EntityCreator);

                return _AttachmentRepository;
            }
        }

        public ICashAccountRepository CashAccountRepository
        {
            get
            {
                if (_CashAccountRepository == null)
                    _CashAccountRepository = new SQLiteCashAccountRepository(_Database);

                return _CashAccountRepository;
            }
        }

        public IParcelAuditRepository ParcelAuditRepository
        {
            get
            {
                if (_ParcelAuditRepository == null)
                    _ParcelAuditRepository = new SQLiteParcelAuditRepository(_Database);

                return _ParcelAuditRepository;
            }
        }

        public IStockSettingRepository StockSettingRepository
        {
            get
            {
                if (_StockSettingRepository == null)
                    _StockSettingRepository = new SQLiteStockSettingRepository(_Database);

                return _StockSettingRepository;
            }
        }

        public IDRPCashBalanceRepository DRPCashBalanceRepository
        {
            get
            {
                if (_DRPCashBalanceRepository == null)
                    _DRPCashBalanceRepository = new SQLiteDRPCashBalanceRepository(_Database);

                return _DRPCashBalanceRepository;
            }
        }

        protected internal SQLitePortfolioUnitOfWork(SQLitePortfolioDatabase database)
        {
            _Database = database;
            _Database._Transaction.BeginTransaction();
            _EntityCreator = new Portfolios.SQLitePortfolioEntityCreator(database);
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
