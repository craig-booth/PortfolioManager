using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Memory.Stocks
{
    class MemoryStockUnitOfWork : IStockUnitOfWork
    {
        private MemoryStockDatabase _Database;
        MemoryStockRepository _StockRepository;
        MemoryCorporateActionRepository _CorporateActionRepository;
        MemoryRelateNTARepository _RelativeNTARepository;
        MemoryStockPriceRepository _StockPriceRepository; 

        public IStockRepository StockRepository
        {
            get
            {
                if (_StockRepository == null)
                    _StockRepository = new MemoryStockRepository(_Database);

                return _StockRepository;
            }
        }

        public ICorporateActionRepository CorporateActionRepository
        {
            get
            {
                if (_CorporateActionRepository == null)
                    _CorporateActionRepository = new MemoryCorporateActionRepository(_Database);

                return _CorporateActionRepository;
            }
        }

        public IRelativeNTARepository RelativeNTARepository
        {
            get
            {
                if (_RelativeNTARepository == null)
                    _RelativeNTARepository = new MemoryRelateNTARepository(_Database);

                return _RelativeNTARepository;
            }
        }


        public IStockPriceRepository StockPriceRepository
        {
            get
            {
                if (_StockPriceRepository == null)
                    _StockPriceRepository = new MemoryStockPriceRepository(_Database);

                return _StockPriceRepository;
            }
        }

        protected internal MemoryStockUnitOfWork(MemoryStockDatabase database)
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
