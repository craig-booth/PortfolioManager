using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Memory.Stocks
{
    class MemoryStockPriceRepository: IStockPriceRepository 
    {
        private MemoryStockDatabase _Database;

        protected internal MemoryStockPriceRepository(MemoryStockDatabase database)
        {
            _Database = database;
        }

        public decimal Get(Guid stockId, DateTime date)
        {
            return 0.00m;
        }

        public void Add(Guid stockId, DateTime date, decimal price)
        {
        }

        public void Update(Guid stockId, DateTime date, decimal price)
        {
        }

        public void Delete(Guid stockId, DateTime date)
        {
        }
    }
}
