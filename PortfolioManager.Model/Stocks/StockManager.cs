using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Stocks
{
    public class StockManager
    {
        private IStockDatabase _Database;

        public StockManager(IStockDatabase database)
        {
            _Database = database;
        }

        public Stock AddStock(string asxCode, string name)
        {
            return AddStock(asxCode, name, DateTimeConstants.NoStartDate());
        }

        public Stock AddStock(string asxCode, string name, DateTime fromDate)
        {
            var stock = new Stock(_Database,fromDate, asxCode, name, StockType.Ordinary, Guid.Empty);

            using (IStockUnitOfWork unitOfWork =  _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }

            return stock;
        }

        public Stock AddStock(string asxCode, string name, StockType type)
        {
            return AddStock(asxCode, name, DateTimeConstants.NoStartDate(), type);
        }

        public Stock AddStock(string asxCode, string name, DateTime fromDate, StockType type)
        {
            var stock = new Stock(_Database, fromDate, asxCode, name, type, Guid.Empty);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }

            return stock;
        }

        public Stock AddStock(string asxCode, string name, StockType type, Stock parent)
        {
            return AddStock(asxCode, name, DateTimeConstants.NoStartDate(), type, parent);
        }

        public Stock AddStock(string asxCode, string name, DateTime fromDate, StockType type, Stock parent)
        {
            var stock = new Stock(_Database, fromDate, asxCode, name, type, parent.Id);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }

            return stock;
        }

        public Stock GetStock(string asxCode)
        {
            return _Database.StockQuery.GetByASXCode(asxCode);
        }

        public IReadOnlyCollection<Stock> GetStocks(DateTime atDate)
        {
            return _Database.StockQuery.GetAll(atDate);
        }

        public string GetASXCode(Guid stockId)
        {
            return _Database.StockQuery.GetASXCode(stockId);
        }

        public string GetASXCode(Guid stockId, DateTime atDate)
        {
            return _Database.StockQuery.GetASXCode(stockId, atDate);
        }

        public IReadOnlyCollection<ICorporateAction> GetCorporateActions(Guid stock, DateTime fromDate, DateTime toDate)
        {
            return _Database.CorporateActionQuery.Find(stock, fromDate, toDate);
        }



    }
}
