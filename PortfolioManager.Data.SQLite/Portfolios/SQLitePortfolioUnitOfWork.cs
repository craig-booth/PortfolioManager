﻿using System;
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
        private SQLiteCashAccountRepository _CashAccountRepository;
        private SQLiteParcelAuditRepository _ParcelAuditRepository;
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
