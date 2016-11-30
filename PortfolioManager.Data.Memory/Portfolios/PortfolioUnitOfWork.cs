using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Memory.Portfolios
{
    public class MemoryPortfolioUnitOfWork : IPortfolioUnitOfWork 
    {
        private MemoryPortfolioDatabase _Database;
        private MemoryParcelRepository _ParcelRepository;
        private MemoryTransactionRepository _TransactionRepository;
        private MemoryCGTEventRepository _CGTEventRepository;

        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (_TransactionRepository == null)
                    _TransactionRepository = new MemoryTransactionRepository(_Database);

                return _TransactionRepository;
            }
        }

        public IParcelRepository ParcelRepository
        {
            get
            {
                if (_ParcelRepository == null)
                    _ParcelRepository = new MemoryParcelRepository(_Database);

                return _ParcelRepository;
            }
        }

        public ICGTEventRepository CGTEventRepository
        {
            get
            {
                if (_CGTEventRepository == null)
                    _CGTEventRepository = new MemoryCGTEventRepository(_Database);

                return _CGTEventRepository;
            }
        }

        public IAttachmentRepository AttachmentRepository
        {
            get
            {
                return null;
            }
        }

        public ICashAccountRepository CashAccountRepository
        {
            get
            {
                return null;
            }
        }

        public IParcelAuditRepository ParcelAuditRepository
        {
            get
            {
                return null;
            }
        }

        public IDRPCashBalanceRepository DRPCashBalanceRepository
        {
            get
            {
                return null;
            }
        }

        protected internal MemoryPortfolioUnitOfWork(MemoryPortfolioDatabase database)
        {
            _Database = database;
        }

        public void Save()
        {

        }

        public void Dispose()
        {

        }
    }

    

}
