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

        public IReadOnlyCollection<ShareHolding> GetHoldings()
        {
            return GetHoldings(DateTime.Now);
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
            return _PortfolioDatabase.PortfolioQuery.GetIncomeReceived(this.Id, fromDate, toDate);
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

        public List<ITransaction> CreateTransactionListForAction(ICorporateAction action)
        {
            if (action is Dividend)
                return CreateTransactionListForAction(action as Dividend);
            else if (action is CapitalReturn)
                return CreateTransactionListForAction(action as CapitalReturn);
            else if (action is Transformation)
                return CreateTransactionListForAction(action as Transformation);
            else
                return null;
        }

        public List<ITransaction> CreateTransactionListForAction(Dividend dividend)
        {
            var transactions = new List<ITransaction>();

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, dividend.Stock, dividend.ActionDate);
            if (parcels.Count == 0)
                return transactions;

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = unitsHeld * dividend.DividendAmount;
            var franked = Math.Round(amountPaid * dividend.PercentFranked, 2, MidpointRounding.AwayFromZero);
            var unFranked = Math.Round(amountPaid * (1 - dividend.PercentFranked), 2, MidpointRounding.AwayFromZero);
            var frankingCredits = Math.Round(((amountPaid / (1 - dividend.CompanyTaxRate)) - amountPaid) * dividend.PercentFranked, 2, MidpointRounding.AwayFromZero);

            /* add drp shares */
            if (dividend.DRPPrice != 0.00M)
            {
                int drpUnits = (int)Math.Round(amountPaid / dividend.DRPPrice);

                transactions.Add(new OpeningBalance()
                    {
                        TransactionDate = dividend.PaymentDate,
                        ASXCode = _StockDatabase.StockQuery.Get(dividend.Stock).ASXCode,
                        Units = drpUnits,
                        CostBase = amountPaid,
                        Comment = "DRP"
                    }
                );
            }

            transactions.Add(new IncomeReceived()
            {
                ASXCode = _StockDatabase.StockQuery.Get(dividend.Stock).ASXCode,
                TransactionDate = dividend.PaymentDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits
            });

            return transactions;
        }

        public List<ITransaction> CreateTransactionListForAction(CapitalReturn capitalReturn)
        {
            var transactions = new List<ITransaction>();

            transactions.Add(new ReturnOfCapital()
                {
                    ASXCode = _StockDatabase.StockQuery.Get(capitalReturn.Stock).ASXCode,
                    TransactionDate = capitalReturn.PaymentDate,
                    Amount = capitalReturn.Amount
                }
            );

            return transactions;
        }

        public List<ITransaction> CreateTransactionListForAction(Transformation transformation)
        {
            var transactions = new List<ITransaction>();

            /* locate parcels that the transformation applies to */
            var ownedParcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, transformation.Stock, transformation.ActionDate);
            if (ownedParcels.Count == 0)
                return transactions;

            int totalUnits = ownedParcels.Sum(x => x.Units);
            decimal totalCostBase = ownedParcels.Sum(x => x.CostBase);

            /* create parcels for resulting stock */
            foreach (ResultingStock resultingStock in transformation.ResultingStocks)
            {
                int units = (int)Math.Round(totalUnits * ((decimal)resultingStock.NewUnits / (decimal)resultingStock.OriginalUnits));
                decimal costBase = totalCostBase * resultingStock.CostBasePercentage;
                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = transformation.ImplementationDate,
                    ASXCode = _StockDatabase.StockQuery.Get(resultingStock.Stock).ASXCode,
                    Units = units,
                    CostBase = costBase,
                    Comment = transformation.Description
                });
            }

            /* Reduce the costbase of the original parcels */
            if (transformation.ResultingStocks.Count > 0)
            {
                decimal originalCostBasePercentage = 1 - transformation.ResultingStocks.Sum(x => x.CostBasePercentage);
                foreach (ShareParcel parcel in ownedParcels)
                {
                    transactions.Add(new CostBaseAdjustment()
                    {
                        TransactionDate = transformation.ImplementationDate,
                        ASXCode = _StockDatabase.StockQuery.Get(transformation.Stock).ASXCode,
                        Percentage = originalCostBasePercentage,
                        Comment = transformation.Description
                    });
                }
            }

            /* Handle disposal of original parcels */
            if (transformation.CashComponent > 0)
            {
                transactions.Add(new Disposal()
                {
                    TransactionDate = transformation.ImplementationDate,
                    ASXCode = _StockDatabase.StockQuery.Get(transformation.Stock).ASXCode,
                    Units = ownedParcels.Sum(x => x.Units),
                    AveragePrice = transformation.CashComponent,
                    TransactionCosts = 0.00M,
                    CGTMethod = CGTCalculationMethod.FirstInFirstOut,
                    Comment = transformation.Description
                });
            }

            return transactions;
        }

        public void ApplyTransaction(ITransaction transaction)
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
        }

        public void ApplyTransaction(Aquisition aquisition)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(aquisition.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(aquisition);

                decimal costBase = aquisition.Units * aquisition.AveragePrice;
                decimal amountPaid = costBase + aquisition.TransactionCosts;

                var parcel = new ShareParcel(_PortfolioDatabase, aquisition.TransactionDate, stock.Id, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase, ParcelEvent.Aquisition);
                AddParcel(parcel);

                unitOfWork.Save();
            }

        }

        public void ApplyTransaction(Disposal disposal)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(disposal.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(disposal);

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


        public void ApplyTransaction(OpeningBalance openingBalance)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(openingBalance.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(openingBalance);

                var parcel = new ShareParcel(_PortfolioDatabase, openingBalance.TransactionDate, stock.Id, openingBalance.Units, openingBalance.CostBase / openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase, ParcelEvent.OpeningBalance);
                AddParcel(parcel);

                unitOfWork.Save();
            }
        }

        public void ApplyTransaction(CostBaseAdjustment costBaseAdjustment)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(costBaseAdjustment.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(costBaseAdjustment);

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

        public void ApplyTransaction(ReturnOfCapital returnOfCapital)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(returnOfCapital.ASXCode);

            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(returnOfCapital);

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

        public void ApplyTransaction(IncomeReceived incomeReceived)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Add(incomeReceived);

                unitOfWork.Save();
            }
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
    }
}
