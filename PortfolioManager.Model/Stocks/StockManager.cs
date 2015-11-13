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
        internal IStockDatabase _Database;

        public StockManager(IStockDatabase database)
        {
            _Database = database;
        }

        public Stock Add(string asxCode, string name)
        {
            return Add(asxCode, name, DateTimeConstants.NoStartDate());
        }

        public Stock Add(string asxCode, string name, DateTime fromDate)
        {
            var stock = new Stock(_Database, fromDate, asxCode, name, StockType.Ordinary, Guid.Empty);

            using (IStockUnitOfWork unitOfWork =  _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }

            return stock;
        }

        public Stock Add(string asxCode, string name, StockType type)
        {
            return Add(asxCode, name, DateTimeConstants.NoStartDate(), type);
        }

        public Stock Add(string asxCode, string name, DateTime fromDate, StockType type)
        {
            var stock = new Stock(_Database, fromDate, asxCode, name, type, Guid.Empty);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }

            return stock;
        }

        public Stock Add(string asxCode, string name, StockType type, Stock parent)
        {
            return Add(asxCode, name, DateTimeConstants.NoStartDate(), type, parent);
        }

        public Stock Add(string asxCode, string name, DateTime fromDate, StockType type, Stock parent)
        {
            var stock = new Stock(_Database, fromDate, asxCode, name, type, parent.Id);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }


            return stock;
        }

        public void Delete(Stock stock)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Delete(stock);
                unitOfWork.Save();
            }

        }

        public void Delete(IEnumerable<Stock> stocks)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (Stock stock in stocks)
                {
                    unitOfWork.StockRepository.Delete(stock);
                }

                unitOfWork.Save();
            }
        }

        public Stock GetStock(Guid id)
        {
            return _Database.StockQuery.Get(id, DateTime.Today);
        }

        public Stock GetStock(Guid id, DateTime atDate)
        {
            return _Database.StockQuery.Get(id, atDate);
        }

        public Stock GetStock(string asxCode)
        {
            return _Database.StockQuery.GetByASXCode(asxCode, DateTime.Today);
        }

        public Stock GetStock(string asxCode, DateTime atDate)
        {
            return _Database.StockQuery.GetByASXCode(asxCode, atDate);
        }

        public IReadOnlyCollection<Stock> GetStocks()
        {
            return _Database.StockQuery.GetAll();
        }

        public IReadOnlyCollection<Stock> GetStocks(DateTime atDate)
        {
            return _Database.StockQuery.GetAll(atDate);
        }

        public string GetASXCode(Guid stockId)
        {
            return _Database.StockQuery.GetASXCode(stockId, DateTime.Today);
        }

        public string GetASXCode(Guid stockId, DateTime atDate)
        {
            return _Database.StockQuery.GetASXCode(stockId, atDate);
        }

        public void ImportStockPrices(string fileName)
        {
            var importer = new StockEasyPriceImporter(fileName);

            using (var unitOfWork = _Database.CreateUnitOfWork())
            {
                importer.ImportToDatabase(_Database.StockQuery, unitOfWork);

                unitOfWork.Save();
            }      
        }

    }
}
