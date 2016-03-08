using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.Memory.Stocks
{
    public class MemoryStockDatabase: IStockDatabase
    {
        internal List<Stock> _Stocks;
        internal List<CorporateAction> _CorporateActions;
        internal List<RelativeNTA> _RelativeNTAs;

        public IStockUnitOfWork CreateUnitOfWork()
        {
            return new MemoryStockUnitOfWork(this);
        }

        public IStockQuery StockQuery {get; private set;}
        public ICorporateActionQuery CorporateActionQuery { get; private set; }

        public MemoryStockDatabase()
        {
            StockQuery = new MemoryStockQuery(this);
            CorporateActionQuery = new MemoryCorporateActionQuery(this);

            _Stocks = new List<Stock>();
            _CorporateActions = new List<CorporateAction>();
            _RelativeNTAs = new List<RelativeNTA>();
        }
    }

}
