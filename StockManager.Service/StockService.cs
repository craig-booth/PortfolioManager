using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace StockManager.Service
{
    public class StockService
    {

        private IStockDatabase _Database;

        public StockService(IStockDatabase database)
        {
            _Database = database;
        }

        public Stock Add(string asxCode, string name)
        {
            var stock = new Stock(DateTimeConstants.NoStartDate, asxCode, name, StockType.Ordinary, Guid.Empty);
            Add(stock);
            return stock;
        }

        public Stock Add(string asxCode, string name, DateTime fromDate)
        {
            var stock = new Stock(fromDate, asxCode, name, StockType.Ordinary, Guid.Empty);
            Add(stock);
            return stock;
        }

        public Stock Add(string asxCode, string name, StockType type)
        {
            var stock = new Stock(DateTimeConstants.NoStartDate, asxCode, name, type, Guid.Empty);
            Add(stock);
            return stock;
        }

        public Stock Add(string asxCode, string name, DateTime fromDate, StockType type)
        {
            var stock = new Stock(fromDate, asxCode, name, type, Guid.Empty);
            Add(stock);
            return stock;
        }

        public Stock Add(string asxCode, string name, StockType type, Stock parent)
        {
            var stock = new Stock(DateTimeConstants.NoStartDate, asxCode, name, type, parent.Id);
            Add(stock);
            return stock;
        }

        public Stock Add(string asxCode, string name, DateTime fromDate, StockType type, Stock parent)
        {
            var stock = new Stock(fromDate, asxCode, name, type, parent.Id);
            Add(stock);
            return stock;
        }

        public void Add(Stock stock)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockRepository.Add(stock);
                unitOfWork.Save();
            }
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

        public void ChangeASXCode(Stock stock, DateTime atDate, string newAsxCode, string newName)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                ModifyStock(unitOfWork, stock, atDate, x => { x.ASXCode = newAsxCode; x.Name = newName; });
                unitOfWork.Save();
            }
            
        }

        public void Delist(Stock stock, DateTime atDate)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                stock.ToDate = atDate;
                unitOfWork.StockRepository.Update(stock);

                unitOfWork.Save();
            }
        }

        public void AddChildStock(Stock stock, DateTime atDate, Stock child)
        {
            if (stock.Type != StockType.StapledSecurity)
                throw new NotStapledSecurityException(stock.ASXCode);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                ModifyStock(_Database.CreateUnitOfWork(), child, atDate, x => { x.ParentId = stock.Id; });
                unitOfWork.Save();
            }
        }

        public void AddChildStocks(Stock stock, DateTime atDate, IEnumerable<Stock> children)
        {
            if (stock.Type != StockType.StapledSecurity)
                throw new NotStapledSecurityException(stock.ASXCode);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (var child in children)
                    ModifyStock(unitOfWork, child, atDate, x => { x.ParentId = stock.Id; });

                unitOfWork.Save();
            }

            
        }

        public IReadOnlyCollection<Stock> GetChildStocks(Stock stock)
        {
            return _Database.StockQuery.GetChildStocks(stock.Id, DateTime.Today);
        }

        public IReadOnlyCollection<Stock> GetChildStocks(Stock stock, DateTime atDate)
        {
            return _Database.StockQuery.GetChildStocks(stock.Id, atDate);
        }

        public void RemoveChildStock(Stock stock, DateTime atDate, Stock child)
        {
            if (stock.Type != StockType.StapledSecurity)
                throw new NotStapledSecurityException(stock.ASXCode);

            if (child.ParentId != stock.Id)
                throw new RecordNotFoundException(child.Id);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                ModifyStock(_Database.CreateUnitOfWork(), child, atDate, x => { x.ParentId = Guid.Empty; });
                unitOfWork.Save();
            }
            
        }

        public void RemoveChildStocks(Stock stock, DateTime atDate, IEnumerable<Stock> children)
        {
            if (stock.Type != StockType.StapledSecurity)
                throw new NotStapledSecurityException(stock.ASXCode);

            // Check children 
            foreach (var child in children)
            {
                if (child.ParentId != stock.Id)
                    throw new RecordNotFoundException(child.Id);
            }

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                foreach (var child in children)
                    ModifyStock(unitOfWork, child, atDate, x => { x.ParentId = Guid.Empty; });

                unitOfWork.Save();
            }

        }

        public decimal PercentageOfParentCostBase(Stock stock, DateTime atDate)
        {
            if (stock.ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(stock.ASXCode);

            return _Database.StockQuery.PercentOfParentCost(stock.ParentId, stock.Id, atDate);
        }

        public IReadOnlyCollection<RelativeNTA> GetRelativeNTAs(Stock stock)
        {
            return _Database.StockQuery.GetRelativeNTAs(stock.ParentId, stock.Id);
        }

        public RelativeNTA AddRelativeNTA(Stock stock, DateTime atDate, decimal percentage)
        {
            RelativeNTA nta;

            if (stock.ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(stock.ASXCode);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                nta = new RelativeNTA(atDate, stock.ParentId, stock.Id, percentage);
                unitOfWork.RelativeNTARepository.Add(nta);

                unitOfWork.Save();

            }

            return nta;
        }
        
        public void UpdateRelativeNTA(Stock stock, DateTime atDate, decimal newPercentage)
        {
            if (stock.ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(stock.ASXCode);

            var nta = _Database.StockQuery.GetRelativeNTA(stock.ParentId, stock.Id, atDate);
            nta.Percentage = newPercentage;

            UpdateRelativeNTA(nta);
        }

        public void UpdateRelativeNTA(RelativeNTA nta)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.RelativeNTARepository.Update(nta);
                unitOfWork.Save();
            }
        }

        public void DeleteRelativeNTA(Stock stock, DateTime atDate)
        {
            if (stock.ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(stock.ASXCode);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.RelativeNTARepository.Delete(stock.ParentId, stock.Id, atDate);

                unitOfWork.Save();
            }
        }

        public void DeleteRelativeNTA(RelativeNTA nta)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.RelativeNTARepository.Delete(nta.Id);

                unitOfWork.Save();
            }
        }

        private void ModifyStock(IStockUnitOfWork unitOfWork, Stock stock, DateTime changeDate, Action<Stock> change)
        {
            if (stock.FromDate == changeDate)
            {
                change(stock);
                unitOfWork.StockRepository.Update(stock);
            }
            else
            {
                /* Update old effective dated record */
                stock.ToDate = changeDate.AddDays(-1);
                unitOfWork.StockRepository.Update(stock);

                var newStock = stock.Clone();
                newStock.FromDate = changeDate;
                newStock.ToDate = DateTimeConstants.NoEndDate;
                change(newStock);

                /* Add new record */
                unitOfWork.StockRepository.Add(newStock);
            }

        }
    }
}
