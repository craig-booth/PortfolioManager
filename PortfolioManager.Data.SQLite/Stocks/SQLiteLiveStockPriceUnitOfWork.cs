using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteLiveStockPriceUnitOfWork : ILiveStockPriceUnitOfWork
    {
        private SQLiteLiveStockPriceDatabase _Database;

        SQLiteLiveStockPriceRepository _LivePriceRepository;

        public ILiveStockPriceRepository LivePriceRepository
        {
            get
            {
                if (_LivePriceRepository == null)
                    _LivePriceRepository = new SQLiteLiveStockPriceRepository(_Database);

                return _LivePriceRepository;
            }
        }

        protected internal SQLiteLiveStockPriceUnitOfWork(SQLiteLiveStockPriceDatabase database)
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
