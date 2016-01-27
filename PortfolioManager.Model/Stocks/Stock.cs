using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public enum StockType {Ordinary, StapledSecurity, Trust}

    [Serializable]
    public class NotStapledSecurityException : Exception
    {
        public NotStapledSecurityException(string asxCode)
            : base(asxCode + " is not a stapled security.")
        {
        }
    }

    [Serializable]
    public class NotStapledSecurityComponentException : Exception
    {
        public NotStapledSecurityComponentException(string asxCode)
            : base(asxCode + " is not a component stock of a stapled security.")
        {
        }
    }


    public class Stock: IEffectiveDatedEntity 
    {
        private IStockDatabase _Database;

        public Guid Id { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public string ASXCode { get; private set; }
        public string Name { get; private set; }
        public StockType Type { get; private set; }
        public Guid ParentId { get; private set; }
        public RoundingRule DividendRoundingRule { get; private set; }

        private decimal _CurrentPrice;
        public decimal CurrentPrice
        {
            get
            {
                if (_CurrentPrice == 0.00m)
                {
                    _CurrentPrice = _Database.StockQuery.GetClosingPrice(Id, DateTime.Today);
                }

                return _CurrentPrice;
            }
        }

        public decimal GetPrice(DateTime atDate)
        {
            return _Database.StockQuery.GetClosingPrice(Id, atDate);
        }

        public void AddPrice(DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Add(Id, atDate, price);
                unitOfWork.Save();
            }
        }

        public void ChangePrice(DateTime atDate, decimal price)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.StockPriceRepository.Update(Id, atDate, price);
                unitOfWork.Save();
            }
        }

        public decimal PercentageOfParentCostBase(DateTime atDate)
        {
            if (ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(ASXCode);
           
            return _Database.StockQuery.PercentOfParentCost(this.ParentId, this.Id, atDate);
        }

        public Stock(IStockDatabase database, DateTime fromDate, string asxCode, string name, StockType type, Guid parent)
            : this(database, Guid.NewGuid(), fromDate, DateTimeConstants.NoEndDate, asxCode, name, type, parent)

        {
        }

        public Stock(IStockDatabase database, Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, Guid parent)
            : this(database, id, fromDate, toDate, asxCode, name, type, parent, RoundingRule.Round)
        {
        }

        public Stock(IStockDatabase database, Guid id, DateTime fromDate, DateTime toDate, string asxCode, string name, StockType type, Guid parent, RoundingRule dividendRoundingRule)
        {
            _Database = database;
            Id = id;
            FromDate = fromDate;
            ToDate = toDate;
            ASXCode = asxCode;
            Name = name;
            FromDate = fromDate;
            Type = type;
            ParentId = parent;
            DividendRoundingRule = dividendRoundingRule;
        }

        public override string ToString()
        {
            return ASXCode + " - " + Name;
        }

        public void ChangeASXCode(DateTime atDate, string newAsxCode, string newName)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                /* Update old effective dated record */
                ToDate = atDate.AddDays(-1);
                unitOfWork.StockRepository.Update(this);

                /* Add new record */
                var newStock = new Stock(_Database, Id, atDate, DateTimeConstants.NoEndDate, newAsxCode, newName, Type, ParentId);
                unitOfWork.StockRepository.Add(newStock);

                unitOfWork.Save();
            }
        }

        public void Delist(DateTime atDate)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {               
                ToDate = atDate.AddDays(-1);
                unitOfWork.StockRepository.Update(this);

                unitOfWork.Save();
            }
        }

        public void AddChildStock(Stock child)
        {
            if (Type != StockType.StapledSecurity)
                throw new NotStapledSecurityException(ASXCode); 

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                child.ParentId = this.Id;
                unitOfWork.StockRepository.Update(child);

                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<Stock> GetChildStocks()
        {
            return _Database.StockQuery.GetChildStocks(this.Id, DateTime.Today);
        }

        public IReadOnlyCollection<Stock> GetChildStocks(DateTime atDate)
        {
            return _Database.StockQuery.GetChildStocks(this.Id, atDate);
        }

        public void RemoveChildStock(Stock child)
        {
            if (Type != StockType.StapledSecurity)
                throw new NotStapledSecurityException(ASXCode);

            if (child.ParentId != Id)
                throw new RecordNotFoundException(child.Id);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                child.ParentId = Guid.Empty;
                unitOfWork.StockRepository.Update(child);

                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<RelativeNTA> GetRelativeNTAs()
        {
            return _Database.StockQuery.GetRelativeNTAs(ParentId, Id);
        }

        public RelativeNTA AddRelativeNTA(DateTime atDate, decimal percentage)
        {
            RelativeNTA nta;

            if (ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(ASXCode);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                nta = new RelativeNTA(_Database, atDate, ParentId, Id, percentage);
                unitOfWork.RelativeNTARepository.Add(nta);

                unitOfWork.Save();

            }

            return nta;

        }

        public void ChangeRelativeNTA(DateTime atDate, decimal newPercentage)
        {
            RelativeNTA nta;

            if (ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(ASXCode);

            nta = _Database.StockQuery.GetRelativeNTA(ParentId, Id, atDate);
            nta.ChangePercentage(newPercentage);
        }

        public void DeleteRelativeNTA(DateTime atDate)
        {

            if (ParentId == Guid.Empty)
                throw new NotStapledSecurityComponentException(ASXCode);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.RelativeNTARepository.Delete(ParentId, Id, atDate);

                unitOfWork.Save();
            }
        }

        public CapitalReturn AddCapitalReturn(DateTime actionDate, DateTime paymentDate, decimal amount, string description)
        {
            CapitalReturn captitalReturn;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                captitalReturn = new CapitalReturn(_Database, Id, actionDate, paymentDate, amount, description);
                unitOfWork.CorporateActionRepository.Add(captitalReturn);

                unitOfWork.Save();

            }

            return captitalReturn;
        }

        public SplitConsolidation AddSplitConsolidation(DateTime actionDate, int oldUnits, int newUnits, string description)
        {
            SplitConsolidation splitConsolidation;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                splitConsolidation = new SplitConsolidation(_Database, Id, actionDate, oldUnits, newUnits, description);
                unitOfWork.CorporateActionRepository.Add(splitConsolidation);

                unitOfWork.Save();

            }

            return splitConsolidation;
        }

        public Dividend AddDividend(DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, string description)
        {
            return AddDividend(actionDate, paymentDate, amount, percentFranked, companyTaxRate, 0.00M, description);
        }

        public Dividend AddDividend(DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, decimal drpPrice, string description)
        {
            Dividend dividend;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                dividend = new Dividend(_Database, Id, actionDate, paymentDate, amount, percentFranked, companyTaxRate, drpPrice, description);
                unitOfWork.CorporateActionRepository.Add(dividend);

                unitOfWork.Save();

            }


            return dividend;
        }

        public Transformation AddTransformation(DateTime actionDate, DateTime implementationDate, decimal cashComponent, string description)
        {
            Transformation transformation;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                transformation = new Transformation(_Database, Id, actionDate, implementationDate, cashComponent, true, description);
                unitOfWork.CorporateActionRepository.Add(transformation);

                unitOfWork.Save();

            }


            return transformation;
        }

        public IReadOnlyCollection<ICorporateAction> GetCorporateActions()
        {
            return _Database.CorporateActionQuery.Find(Id, DateTimeConstants.NoStartDate, DateTimeConstants.NoEndDate);
        }

        public IReadOnlyCollection<ICorporateAction> GetCorporateActions(DateTime fromDate, DateTime toDate)
        {
            return _Database.CorporateActionQuery.Find(Id, fromDate, toDate);
        }

        public void DeleteCorporateAction(ICorporateAction corporateAction)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Delete(corporateAction);
                unitOfWork.Save();
            }
        }

        public void DeleteCorporateAction(Guid id)
        {
            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Delete(id);
                unitOfWork.Save();
            }
        }

    }
 
}
