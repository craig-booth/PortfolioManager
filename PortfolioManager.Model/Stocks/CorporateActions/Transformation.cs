using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Stocks
{

    public class Transformation : ICorporateAction
    {
        private IStockDatabase _Database;
        private List<ResultingStock> _ResultingStocks;

        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public DateTime ImplementationDate { get; set; }
        public string Description { get; private set; }
        public decimal CashComponent { get; private set; }
        public IEnumerable<ResultingStock> ResultingStocks 
        { 
            get 
            { 
                return _ResultingStocks.AsEnumerable(); 
            } 
        }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.Transformation;
            }
        }

        public Transformation(IStockDatabase stockDatabase, Guid id, Guid stock, DateTime actionDate, DateTime implementationDate, decimal cashComponent, string description)
        {
            _Database = stockDatabase;
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            ImplementationDate = implementationDate;
            CashComponent = cashComponent;
            Description = description;

            _ResultingStocks = new List<ResultingStock>();
        }

        public Transformation(IStockDatabase stockDatabase, Guid stock, DateTime actionDate, DateTime implementationDate, decimal cashComponent, string description)
            : this(stockDatabase, Guid.NewGuid(), stock, actionDate, implementationDate, cashComponent, description)
        {

        }

        public void Change(DateTime newActionDate, DateTime newImplementationDate, decimal newCashComponent, string newDescription)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                ActionDate = newActionDate;
                ImplementationDate = newImplementationDate;
                CashComponent = newCashComponent;
                Description = newDescription;

                unitOfWork.CorporateActionRepository.Update(this);

                unitOfWork.Save();
            }
        }

        public void AddResultStockInternal(ResultingStock resultingStock)
        {
            _ResultingStocks.Add(resultingStock);
        }

        public void AddResultStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage)
        {
            var resultStock = new ResultingStock(stock, originalUnits, newUnits, costBasePercentage);

            AddResultStockInternal(resultStock);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }

        public void ChangeResultStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage)
        {
            var resultStock = _ResultingStocks.Find(x => x.Stock == stock);

            if (resultStock == null)
                throw new RecordNotFoundException(stock);

            resultStock.Change(originalUnits, newUnits ,costBasePercentage);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }

        public void DeleteResultStock(Guid stock)
        {
            var resultStock = _ResultingStocks.Find(x => x.Stock == stock);

            if (resultStock == null)
                throw new RecordNotFoundException(stock);

            _ResultingStocks.Remove(resultStock);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }
    }

    public class ResultingStock
    {
        public Guid Stock { get; private set; }
        public int OriginalUnits { get; private set; }
        public int NewUnits { get; private set; }
        public decimal CostBasePercentage { get; private set; }

        public ResultingStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage)
        {
            Stock = stock;
            OriginalUnits = originalUnits;
            NewUnits = newUnits;
            CostBasePercentage = costBasePercentage;
        }

        protected internal void Change(int originalUnits, int newUnits, decimal costBasePercentage)
        {
            OriginalUnits = originalUnits;
            NewUnits = newUnits;
            CostBasePercentage = costBasePercentage;
        }

    }

}
