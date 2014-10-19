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

        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public DateTime ImplementationDate { get; set; }
        public string Description { get; private set; }
        public decimal CashComponent { get; private set; }
        public List<ResultingStock> ResultingStocks { get; private set; }

        public Transformation(IStockDatabase stockDatabase, Guid id, Guid stock, DateTime actionDate, DateTime implementationDate, decimal cashComponent, string description)
        {
            _Database = stockDatabase;
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            ImplementationDate = implementationDate;
            CashComponent = cashComponent;
            Description = description;

            ResultingStocks = new List<ResultingStock>();
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

        public void AddResultStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage)
        {
            var resultStock = new ResultingStock(stock, originalUnits, newUnits, costBasePercentage);

            ResultingStocks.Add(resultStock);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }

        public void ChangeResultStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage)
        {
            var resultStock = ResultingStocks.Find(x => x.Stock == stock);

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
            var resultStock = ResultingStocks.Find(x => x.Stock == stock);

            if (resultStock == null)
                throw new RecordNotFoundException(stock);

            ResultingStocks.Remove(resultStock);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(Portfolio forPortfolio)
        {
            var transactions = new List<ITransaction>();

            /* locate parcels that the transformation applies to */
            var ownedParcels = forPortfolio.GetParcels(Stock, ActionDate); 
            if (ownedParcels.Count == 0)
                return transactions;

            int totalUnits = ownedParcels.Sum(x => x.Units);
            decimal totalCostBase = ownedParcels.Sum(x => x.CostBase);

            /* create parcels for resulting stock */
            foreach (ResultingStock resultingStock in ResultingStocks)
            {
                int units = (int)Math.Round(totalUnits * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                decimal costBase = totalCostBase * resultingStock.CostBasePercentage;
                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = ImplementationDate,
                    ASXCode = _Database.StockQuery.Get(resultingStock.Stock).ASXCode,
                    Units = units,
                    CostBase = costBase,
                    Comment = Description
                });
            }

            /* Reduce the costbase of the original parcels */
            if (ResultingStocks.Count > 0)
            {
                decimal originalCostBasePercentage = 1 - ResultingStocks.Sum(x => x.CostBasePercentage);
                foreach (ShareParcel parcel in ownedParcels)
                {
                    transactions.Add(new CostBaseAdjustment()
                    {
                        TransactionDate = ImplementationDate,
                        ASXCode = _Database.StockQuery.Get(this.Stock).ASXCode,
                        Percentage = originalCostBasePercentage,
                        Comment = Description
                    });
                }
            }

            /* Handle disposal of original parcels */
            if (CashComponent > 0)
            {
                transactions.Add(new Disposal()
                {
                    TransactionDate = ImplementationDate,
                    ASXCode = _Database.StockQuery.Get(this.Stock).ASXCode,
                    Units = ownedParcels.Sum(x => x.Units),
                    AveragePrice = CashComponent,
                    TransactionCosts = 0.00M,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    Comment = Description
                });
            }

            return transactions.AsReadOnly();
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
