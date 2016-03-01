using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace StockManager.Service
{
    public class StockPriceService
    {

        private IStockDatabase _Database;

        public StockPriceService(IStockDatabase database)
        {
            _Database = database;
        }

        public decimal GetCurrentPrice(Guid stockId)
        {
            return _Database.StockQuery.GetClosingPrice(stockId, DateTime.Today);
        }

        public decimal GetPrice(Guid stockId, DateTime atDate)
        {
            return _Database.StockQuery.GetClosingPrice(stockId, atDate);
        }

        public void AddPrice(Guid stockId, DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Add(stockId, atDate, price);
                unitOfWork.Save();
            }
        }

        public void ChangePrice(Guid stockId, DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Update(stockId, atDate, price);
                unitOfWork.Save();
            }
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