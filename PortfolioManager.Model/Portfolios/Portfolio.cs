using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{

    public class StockSetting
    {
        public string ASXCode { get; private set; }
        public bool DRPActive { get; set; }

        public StockSetting(string asxCode)
        {
            ASXCode = asxCode;
            DRPActive = false;
        }
    }

    public class Portfolio: IEntity
    {
        private IPortfolioDatabase _PortfolioDatabase;
        private IStockDatabase _StockDatabase;

        public Guid Id { get; private set; }
        public string Name { get; set; }

        public Dictionary<string, StockSetting> StockSetting { get; private set; }

        public IReadOnlyCollection<ShareParcel> GetParcels()
        {
            return GetParcels(DateTime.Now);
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(DateTime atDate)
        {
            var allParcels = _PortfolioDatabase.PortfolioQuery.GetAllParcels(this.Id, atDate);

            return allParcels.Where(x => x.IncludeInParcels == true).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Stock ofStock)
        {
            return GetParcels(ofStock, DateTime.Now);
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Stock ofStock, DateTime atDate)
        {
            var allParcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, ofStock.Id, atDate);

            return allParcels.Where(x => x.IncludeInParcels == true).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Guid ofStock, DateTime atDate)
        {
            var allParcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, ofStock, atDate);

            return allParcels.Where(x => x.IncludeInParcels == true).ToList().AsReadOnly();
        }

        List<ShareHolding> _Holdings;
        public IReadOnlyCollection<ShareHolding> Holdings
        {
            get
            {
                if (_Holdings == null)
                {
                    _Holdings = new List<ShareHolding>(GetHoldings(DateTime.Now));
                }

                return _Holdings.AsReadOnly();
            }
        }

        private void UpdateHoldings(string asxCode)
        {
            if (_Holdings == null)
                return;

            var stock = _StockDatabase.StockQuery.GetByASXCode(asxCode);

            var existingHolding = _Holdings.Where(x => x.Stock.Id == stock.Id);
            if (existingHolding.Count() > 0)
                _Holdings.Remove(existingHolding.First());

            ShareHolding newHolding = GetHoldingsForStock(stock.Id, DateTime.Now);
            if (newHolding != null)
                _Holdings.Add(newHolding);
        }

        public ShareHolding GetHoldingsForStock(Guid stock, DateTime atDate)
        {
            var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(Id, stock, atDate);

            var holdingsQuery = from parcel in parcels
                                where parcel.IncludeInHoldings
                                group parcel by parcel.Stock into parcelGroup
                                select new ShareHolding(_StockDatabase.StockQuery.Get(parcelGroup.Key), parcelGroup.Sum(x => x.Units), parcelGroup.Average(x => x.UnitPrice), parcelGroup.Sum(x => x.Amount));

            if (holdingsQuery.Count() > 0)
                return holdingsQuery.First();
            else
                return null;
        }

        public IReadOnlyCollection<ShareHolding> GetHoldings(DateTime atDate)
        {
            var allParcels = _PortfolioDatabase.PortfolioQuery.GetAllParcels(this.Id, atDate);

            var holdingsQuery = from parcel in allParcels
                                where parcel.IncludeInHoldings
                                group parcel by parcel.Stock into parcelGroup
                                select new ShareHolding(_StockDatabase.StockQuery.Get(parcelGroup.Key), parcelGroup.Sum(x => x.Units), parcelGroup.Average(x => x.UnitPrice), parcelGroup.Sum(x => x.Amount));

            return holdingsQuery.ToList().AsReadOnly(); 
        }

        public IReadOnlyCollection<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetCGTEvents(this.Id, fromDate, toDate);
        } 

        public IReadOnlyCollection<IncomeReceived> GetIncomeReceived(DateTime fromDate, DateTime toDate)
        {
            var incomeTransactions = _PortfolioDatabase.PortfolioQuery.GetTransactions(this.Id, TransactionType.Income, fromDate, toDate);

            var incomeQuery = from income in incomeTransactions
                                select income as IncomeReceived;

            return incomeQuery.ToList().AsReadOnly(); 
        }

        private Portfolio(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        protected internal Portfolio(string name, IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
            : this(name)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;

            StockSetting = new Dictionary<string, StockSetting>();
        }

        public void AddTransactions(IEnumerable<ITransaction> transactions)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (ITransaction transaction in transactions)
                {
                    unitOfWork.TransactionRepository.Add(transaction);
                //    ApplyTransaction(transaction);
                };
                unitOfWork.Save();
            }       
        }

        public void AddTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(transaction);
                unitOfWork.Save();
            }

            ApplyTransaction(transaction);
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Id, fromDate, toDate);
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Id, asxCode, fromDate, toDate);
        }

        private void ApplyTransactions(IEnumerable<ITransaction> transactions)
        {
            foreach (ITransaction transaction in transactions)
            {
                ApplyTransaction(transaction);
            };
        }

        private void ApplyTransaction(ITransaction transaction)
        {
            if (transaction is Aquisition)
                ApplyTransaction(transaction as Aquisition);
            else if (transaction is Disposal)
                ApplyTransaction(transaction as Disposal);
            else if (transaction is OpeningBalance)
                ApplyTransaction(transaction as OpeningBalance);
            else if (transaction is CostBaseAdjustment)
                ApplyTransaction(transaction as CostBaseAdjustment);
            else if (transaction is ReturnOfCapital)
                ApplyTransaction(transaction as ReturnOfCapital);
            else if (transaction is IncomeReceived)
                ApplyTransaction(transaction as IncomeReceived);
            else
                return;

            UpdateHoldings(transaction.ASXCode);
        }

        private void ApplyTransaction(Aquisition aquisition)
        {
            ShareParcel newParcel;

            Stock stock = _StockDatabase.StockQuery.GetByASXCode(aquisition.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                decimal costBase = aquisition.Units * aquisition.AveragePrice;
                decimal amountPaid = costBase + aquisition.TransactionCosts;

                newParcel = new ShareParcel(_PortfolioDatabase, aquisition.TransactionDate, stock.Id, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase, ParcelEvent.Aquisition);
                AddParcel(newParcel);

                unitOfWork.Save();    
            }
        }

        private void ApplyTransaction(Disposal disposal)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(disposal.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                /* Create CGT calculator */
                var CGTCalculator = new CGTCalculator();

                /* Determine which parcels to sell based on CGT method */
                var CGTCalculation = CGTCalculator.CalculateCapitalGain(this, disposal.TransactionDate, stock, disposal.Units, disposal.Units * disposal.AveragePrice, disposal.CGTMethod);

                /* dispose of select parcels */
                foreach (ParcelSold parcelSold in CGTCalculation.ParcelsSold)
                {
                    decimal amountReceived = parcelSold.UnitsSold * disposal.AveragePrice;
                    DisposeOfParcel(parcelSold.Parcel, disposal.TransactionDate, parcelSold.UnitsSold, amountReceived, disposal.Description);
                };

                unitOfWork.Save();
            }
        }


        private void ApplyTransaction(OpeningBalance openingBalance)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(openingBalance.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                var parcel = new ShareParcel(_PortfolioDatabase, openingBalance.TransactionDate, stock.Id, openingBalance.Units, openingBalance.CostBase / openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase, ParcelEvent.OpeningBalance);
                AddParcel(parcel);

                unitOfWork.Save();
            }
        }

        private void ApplyTransaction(CostBaseAdjustment costBaseAdjustment)
        {

            Stock stock = _StockDatabase.StockQuery.GetByASXCode(costBaseAdjustment.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                /* locate parcels that the dividend applies to */
                var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, stock.Id, costBaseAdjustment.TransactionDate);

                /* Reduce cost base of parcels */
                foreach (ShareParcel parcel in parcels)
                {
                    parcel.Modify(costBaseAdjustment.TransactionDate, ParcelEvent.CostBaseReduction, parcel.Units, parcel.CostBase * costBaseAdjustment.Percentage, "");
                }

                unitOfWork.Save();
            }
        }

        private void ApplyTransaction(ReturnOfCapital returnOfCapital)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(returnOfCapital.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                /* locate parcels that the dividend applies to */
                var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, stock.Id, returnOfCapital.TransactionDate);

                /* Reduce cost base of parcels */
                foreach (ShareParcel parcel in parcels)
                {
                    var costBaseReduction = parcel.Units * returnOfCapital.Amount;
                    parcel.Modify(returnOfCapital.TransactionDate, ParcelEvent.CostBaseReduction, parcel.Units, parcel.CostBase - costBaseReduction, "");
                }

                unitOfWork.Save();
            }
        }

        private void ApplyTransaction(IncomeReceived incomeReceived)
        {
            /* No need to do anything */
        }

        private void AddParcel(ShareParcel parcel)
        {  
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                /* Handle Stapled Securities */
                var stock = _StockDatabase.StockQuery.Get(parcel.Stock);
                if (stock.Type == StockType.StapledSecurity)
                {
                    /* Get child stocks */
                    var childStocks = _StockDatabase.StockQuery.GetChildStocks(parcel.Stock);

                    foreach (Stock childStock in childStocks)
                    {
                        decimal percentageOfParent = childStock.PercentageOfParentCostBase(parcel.AquisitionDate);
                        decimal costBase = parcel.CostBase * percentageOfParent;
                        decimal amount = parcel.Amount * percentageOfParent;
                        decimal unitPrice = costBase / parcel.Units;

                        var childParcel = new ShareParcel(_PortfolioDatabase, parcel.AquisitionDate, childStock.Id, parcel.Units, unitPrice, amount, costBase, parcel.Event);
                        childParcel.IncludeInHoldings = false;
                        childParcel.Parent = parcel.Id;

                        unitOfWork.ParcelRepository.Add(childParcel);
                    }
                    parcel.IncludeInParcels = false;
                }

                unitOfWork.ParcelRepository.Add(parcel);

                unitOfWork.Save();
            }
        }

        private void DisposeOfParcel(ShareParcel parcel, DateTime disposalDate, int units, decimal amountReceived, string description)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                decimal costBase;
                CGTEvent cgtEvent;

                /* Handle Stapled Securities */
                var stock = _StockDatabase.StockQuery.Get(parcel.Stock);
                if (stock.Type == StockType.StapledSecurity)
                {
                    /* Get child parcels */
                    var childParcels = _PortfolioDatabase.PortfolioQuery.GetChildParcels(parcel.Id);

                    foreach (ShareParcel childParcel in childParcels)
                    {
                        /* Modify Parcel */
                        costBase = childParcel.CostBase / (units / childParcel.Units);
                        childParcel.Modify(disposalDate, ParcelEvent.Disposal, childParcel.Units - units, childParcel.CostBase - costBase, description);

                        /* Create CGT Event */
                        var childStock = _StockDatabase.StockQuery.Get(childParcel.Stock);
                        decimal percentageOfParent = childStock.PercentageOfParentCostBase(disposalDate);
                        decimal childAmountReceived = amountReceived * percentageOfParent;
                        cgtEvent = new CGTEvent(childParcel.Stock, disposalDate, units, costBase, childAmountReceived);
                        unitOfWork.CGTEventRepository.Add(cgtEvent);
                    }
                }

                /* Modify Parcel */
                costBase = parcel.CostBase / (units / parcel.Units);
                parcel.Modify(disposalDate, ParcelEvent.Disposal, parcel.Units - units, parcel.CostBase - costBase, description);

                /* Create CGT Event */
                if (stock.Type != StockType.StapledSecurity)
                {
                    cgtEvent = new CGTEvent(parcel.Stock, disposalDate, units, costBase, amountReceived);
                    unitOfWork.CGTEventRepository.Add(cgtEvent);
                }

                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<ICorporateAction> GetUnappliedCorparateActions()
        {
            IReadOnlyCollection<ICorporateAction> corporateActions;

            var allCorporateActions = new List<ICorporateAction>();
 
            foreach (ShareHolding holding in Holdings)
            {
                corporateActions = _StockDatabase.CorporateActionQuery.Find(holding.Stock.Id, new DateTime(0001, 01, 01), new DateTime(9999, 12, 31));
                AddUnappliedCorporateActions(allCorporateActions, corporateActions);
                if (holding.Stock.Type == StockType.StapledSecurity)
                {
                    foreach (Stock childStock in holding.Stock.GetChildStocks())
                    {
                        corporateActions = _StockDatabase.CorporateActionQuery.Find(childStock.Id, new DateTime(0001, 01, 01), new DateTime(9999, 12, 31));
                        AddUnappliedCorporateActions(allCorporateActions, corporateActions);
                    }
                }    
            }
           
            /* Sort by Action Date */
            allCorporateActions.Sort(new CorporateActionComparer());

            return allCorporateActions.AsReadOnly();
        }

        private void AddUnappliedCorporateActions(IList<ICorporateAction> toList, IEnumerable<ICorporateAction> fromList)
        {
            
            foreach (ICorporateAction corporateAction in fromList)
            {
                IReadOnlyCollection<ITransaction> transactions;
                TransactionType type;
                DateTime date;
                string asxCode;

                if (corporateAction is Dividend)
                {
                    Dividend dividend = corporateAction as Dividend;
                    date = dividend.PaymentDate;
                    type = TransactionType.Income;
                    asxCode = _StockDatabase.StockQuery.Get(dividend.Stock).ASXCode;
                }
                else if (corporateAction is CapitalReturn)
                {
                    CapitalReturn capitalReturn = corporateAction as CapitalReturn;
                    date = capitalReturn.PaymentDate;
                    type = TransactionType.ReturnOfCapital;
                    asxCode = _StockDatabase.StockQuery.Get(capitalReturn.Stock).ASXCode;
                }
                else if (corporateAction is Transformation)
                {
                    Transformation transformation = corporateAction as Transformation;
                    date = transformation.ImplementationDate;
                    
                    if (transformation.ResultingStocks.Count > 0)
                    {
                        type = TransactionType.OpeningBalance;
                        asxCode = _StockDatabase.StockQuery.Get(transformation.ResultingStocks[0].Stock).ASXCode;
                    }
                    else 
                    {
                        type = TransactionType.Disposal; 
                        asxCode = _StockDatabase.StockQuery.Get(transformation.Stock).ASXCode;
                    }
                    
                } 
                else
                    continue;

                transactions = _PortfolioDatabase.PortfolioQuery.GetTransactions(Id, asxCode, type, date, date);                
                if (transactions.Count() == 0)
                    toList.Add(corporateAction);
            }
        }
    }
}


