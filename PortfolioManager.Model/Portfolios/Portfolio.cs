using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

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
        public CashAccount CashAccount { get; private set; }

        public Dictionary<string, StockSetting> StockSetting { get; private set; }

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

            CashAccount = new CashAccount();

            /* Load transactions */
            var allTransactions = GetTransactions(DateTime.MinValue, DateTime.MaxValue);
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (var transaction in allTransactions)
                    ApplyTransaction(unitOfWork, transaction);
                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<ShareParcel> GetParcels() 
        {
            return GetParcels(DateTime.Now, false);
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(DateTime atDate)
        {
            return GetParcels(atDate, false);
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(DateTime atDate, bool includeHiddenParcels)
        {
            var allParcels = _PortfolioDatabase.PortfolioQuery.GetAllParcels(this.Id, atDate);

            if (includeHiddenParcels)
                return allParcels.ToList().AsReadOnly();
            else
                return allParcels.Where(x => x.IncludeInParcels == true).ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Stock ofStock)
        {
            return GetParcels(ofStock, DateTime.Now);
        }


        public IReadOnlyCollection<ShareParcel> GetParcels(Guid ofStock, DateTime atDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, ofStock, atDate);
        }

        public IReadOnlyCollection<ShareParcel> GetParcels(Stock ofStock, DateTime atDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, ofStock.Id, atDate);
        }

        public ShareHolding GetHoldingsForStock(Guid stock, DateTime atDate)
        {
            var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(Id, stock, atDate);

            var holdingsQuery = from parcel in parcels
                                where parcel.IncludeInHoldings == true
                                group parcel by parcel.Stock into parcelGroup
                                select new ShareHolding(_StockDatabase.StockQuery.Get(parcelGroup.Key, atDate), parcelGroup.Sum(x => x.Units), parcelGroup.Average(x => x.UnitPrice), parcelGroup.Sum(x => x.Amount));

            if (holdingsQuery.Count() > 0)
                return holdingsQuery.First();
            else
                return null;
        }

        public IReadOnlyCollection<ShareHolding> GetHoldings(DateTime atDate)
        {
            var allParcels = _PortfolioDatabase.PortfolioQuery.GetAllParcels(this.Id, atDate);

            var holdingsQuery = from parcel in allParcels
                                where parcel.IncludeInHoldings == true
                                group parcel by parcel.Stock into parcelGroup
                                select new ShareHolding(_StockDatabase.StockQuery.Get(parcelGroup.Key, atDate), parcelGroup.Sum(x => x.Units), parcelGroup.Average(x => x.UnitPrice), parcelGroup.Sum(x => x.Amount));

            return holdingsQuery.OrderBy(x => x.Stock.ASXCode).ToList().AsReadOnly(); 
        }

        public IReadOnlyCollection<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetCGTEvents(this.Id, fromDate, toDate);
        } 

        public IReadOnlyCollection<IncomeReceived> GetIncomeReceived(DateTime fromDate, DateTime toDate)
        {         
            return _PortfolioDatabase.PortfolioQuery.GetIncome (this.Id, fromDate, toDate);
        }


        public IReadOnlyCollection<ITransaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Id, fromDate, toDate);
        }

        public IReadOnlyCollection<ITransaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetTransactions(Id, asxCode, fromDate, toDate);
        }

        public void ProcessTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                ApplyTransaction(unitOfWork, transaction);
                unitOfWork.TransactionRepository.Add(transaction);
                unitOfWork.Save();
            }            
        }

        public void ProcessTransactions(IEnumerable<ITransaction> transactions)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                foreach (ITransaction transaction in transactions)
                {
                    ApplyTransaction(unitOfWork, transaction);
                    unitOfWork.TransactionRepository.Add(transaction);
                    
                };
                unitOfWork.Save();
            }
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Update(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);
        }

        public void DeleteTransaction(ITransaction transaction)
        {
            using (IPortfolioUnitOfWork unitOfWork = _PortfolioDatabase.CreateUnitOfWork())
            {
                unitOfWork.TransactionRepository.Delete(transaction);
                unitOfWork.Save();
            }

            // Need to work out how to reapply
            // _Portfolio.ApplyTransaction(transaction);
        }

        private void ApplyTransaction(IPortfolioUnitOfWork unitOfWork, ITransaction transaction)
        {
            if (transaction.Type == TransactionType.Aquisition)
                ApplyAquisition(unitOfWork, transaction as Aquisition);
            else if (transaction.Type == TransactionType.Disposal)
                ApplyDisposal(unitOfWork, transaction as Disposal);
            else if (transaction.Type == TransactionType.OpeningBalance)
                ApplyOpeningBalance(unitOfWork, transaction as OpeningBalance);
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
                ApplyCostBaseAdjustment(unitOfWork, transaction as CostBaseAdjustment);
            else if (transaction.Type == TransactionType.ReturnOfCapital)
                ApplyReturnOfCapital(unitOfWork, transaction as ReturnOfCapital);
            else if (transaction.Type == TransactionType.Income)
                ApplyIncomeReceived(unitOfWork, transaction as IncomeReceived);
            else if ((transaction.Type == TransactionType.Deposit) ||
                     (transaction.Type == TransactionType.Withdrawl) |
                     (transaction.Type == TransactionType.Fee) ||
                     (transaction.Type == TransactionType.Interest))
                ApplyCashTransaction(unitOfWork, transaction as CashTransaction);
            else
                return;
        }

        private void ApplyAquisition(IPortfolioUnitOfWork unitOfWork, Aquisition aquisition)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(aquisition.ASXCode, aquisition.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(aquisition, "Cannot aquire child securities. Aquire stapled security instead");

            decimal amountPaid = (aquisition.Units * aquisition.AveragePrice) + aquisition.TransactionCosts;
            decimal costBase = amountPaid;

            AddParcel(unitOfWork, aquisition.TransactionDate, stock.Id, aquisition.Units, aquisition.AveragePrice, amountPaid, costBase, ParcelEvent.Aquisition);

            CashAccount.AddTransaction(CashAccountTransactionType.Transfer, aquisition.TransactionDate, String.Format("Purchase of {0}", aquisition.ASXCode), amountPaid); 
        }

        private void ApplyDisposal(IPortfolioUnitOfWork unitOfWork, Disposal disposal)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(disposal.ASXCode, disposal.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(disposal, "Cannot dispose of child securities. Dispose of stapled security instead");

            /* Create CGT calculator */
            var CGTCalculator = new CGTCalculator();

            /* Determine which parcels to sell based on CGT method */
            decimal amountReceived = (disposal.Units * disposal.AveragePrice) - disposal.TransactionCosts;
            var CGTCalculation = CGTCalculator.CalculateCapitalGain(this, disposal.TransactionDate, stock, disposal.Units, amountReceived, disposal.CGTMethod);

            if (CGTCalculation.UnitsSold == 0)
                throw new NoParcelsForTransaction(disposal, "No parcels found for transaction");
            else if (CGTCalculation.UnitsSold < disposal.Units)
                throw new NotEnoughSharesForDisposal(disposal, "Not enough shares for disposal");

            /* dispose of select parcels */
            decimal totalAmount = 0;
            foreach (ParcelSold parcelSold in CGTCalculation.ParcelsSold)
            {
                DisposeOfParcel(unitOfWork, parcelSold.Parcel, disposal.TransactionDate, parcelSold.UnitsSold, parcelSold.AmountReceived, disposal.Description);

                totalAmount += amountReceived;
            };

            CashAccount.AddTransaction(CashAccountTransactionType.Transfer, disposal.TransactionDate, String.Format("Sale of {0}", disposal.ASXCode), totalAmount + disposal.TransactionCosts); 
        }

        private void ApplyOpeningBalance(IPortfolioUnitOfWork unitOfWork, OpeningBalance openingBalance)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(openingBalance.ASXCode, openingBalance.TransactionDate);

            if (stock.ParentId != Guid.Empty)
                throw new TransctionNotSupportedForChildSecurity(openingBalance, "Cannot aquire child securities. Aquire stapled security instead");

            AddParcel(unitOfWork, openingBalance.TransactionDate, stock.Id, openingBalance.Units, openingBalance.CostBase / openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase, ParcelEvent.OpeningBalance);
        }

        private void ApplyCostBaseAdjustment(IPortfolioUnitOfWork unitOfWork, CostBaseAdjustment costBaseAdjustment)
        {

            Stock stock = _StockDatabase.StockQuery.GetByASXCode(costBaseAdjustment.ASXCode, costBaseAdjustment.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(costBaseAdjustment, "Cannot adjust cost base of stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, stock.Id, costBaseAdjustment.TransactionDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(costBaseAdjustment, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            foreach (ShareParcel parcel in parcels)
            {
                var costBaseReduction = parcel.CostBase * (1 - costBaseAdjustment.Percentage);
                ModifyParcel(unitOfWork, parcel, costBaseAdjustment.TransactionDate, ParcelEvent.CostBaseReduction, parcel.Units, parcel.CostBase - costBaseReduction, "");

                // If a child stock then also adjust cost base of parent
                if (stock.ParentId != Guid.Empty)
                {
                    // Get parent parcel
                    var parentParcel = _PortfolioDatabase.PortfolioQuery.GetParcel(stock.ParentId, costBaseAdjustment.TransactionDate);

                    if (parentParcel != null)
                        ModifyParcel(unitOfWork, parentParcel, costBaseAdjustment.TransactionDate, ParcelEvent.CostBaseReduction, parentParcel.Units, parentParcel.CostBase - costBaseReduction, "");
                }
            }

        }

        private void ApplyReturnOfCapital(IPortfolioUnitOfWork unitOfWork, ReturnOfCapital returnOfCapital)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(returnOfCapital.ASXCode, returnOfCapital.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(returnOfCapital, "Cannot have a return of capital for stapled securities. Adjust cost base of child securities instead");

            /* locate parcels that the transaction applies to */
            var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, stock.Id, returnOfCapital.TransactionDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(returnOfCapital, "No parcels found for transaction");

            /* Reduce cost base of parcels */
            decimal totalAmount = 0;
            foreach (ShareParcel parcel in parcels)
            {
                var costBaseReduction = parcel.Units * returnOfCapital.Amount;

                if (costBaseReduction <= parcel.CostBase)
                    ModifyParcel(unitOfWork, parcel, returnOfCapital.TransactionDate, ParcelEvent.CostBaseReduction, parcel.Units, parcel.CostBase - costBaseReduction, "");
                else
                {
                    ModifyParcel(unitOfWork, parcel, returnOfCapital.TransactionDate, ParcelEvent.CostBaseReduction, parcel.Units, 0.00m, "");

                    var cgtEvent = new CGTEvent(parcel.Stock, returnOfCapital.TransactionDate, parcel.Units, parcel.CostBase, costBaseReduction - parcel.CostBase);
                    unitOfWork.CGTEventRepository.Add(cgtEvent);
                }

                totalAmount += costBaseReduction;
            }

            CashAccount.AddTransaction(CashAccountTransactionType.Transfer, returnOfCapital.TransactionDate, String.Format("Return of capital for {0}", returnOfCapital.ASXCode), totalAmount); 

        }

        private void ApplyCashTransaction(IPortfolioUnitOfWork unitOfWork, CashTransaction cashTransaction)
        {
            if (cashTransaction.Type == TransactionType.Withdrawl)
                CashAccount.AddTransaction(CashAccountTransactionType.Withdrawl, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
            else if (cashTransaction.Type == TransactionType.Deposit)
                CashAccount.AddTransaction(CashAccountTransactionType.Deposit, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
            else if (cashTransaction.Type == TransactionType.Fee)
                CashAccount.AddTransaction(CashAccountTransactionType.Fee, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);
            else if (cashTransaction.Type == TransactionType.Interest)
                CashAccount.AddTransaction(CashAccountTransactionType.Interest, cashTransaction.TransactionDate, cashTransaction.Description, cashTransaction.Amount);         
        }

        private void ApplyIncomeReceived(IPortfolioUnitOfWork unitOfWork, IncomeReceived incomeReceived)
        {
            Stock stock = _StockDatabase.StockQuery.GetByASXCode(incomeReceived.ASXCode, incomeReceived.TransactionDate);

            if (stock.Type == StockType.StapledSecurity)
                throw new TransctionNotSupportedForStapledSecurity(incomeReceived, "Cannot have a income for stapled securities. Income should be recorded against child securities instead");

            /* locate parcels that the dividend applies to */
            var parcels = _PortfolioDatabase.PortfolioQuery.GetParcelsForStock(this.Id, stock.Id, incomeReceived.TransactionDate);

            if (parcels.Count == 0)
                throw new NoParcelsForTransaction(incomeReceived, "No parcels found for transaction");

            /* Handle any tax deferred amount recieved */
            if (incomeReceived.TaxDeferred > 0)
            {
                /* Apportion amount between parcels */
                ApportionedValue[] apportionedAmounts = new ApportionedValue[parcels.Count];
                int i = 0;
                foreach (ShareParcel parcel in parcels)
                    apportionedAmounts[i++].Units = parcel.Units;
                MathUtils.ApportionAmount(incomeReceived.TaxDeferred, apportionedAmounts);

                /* Reduce cost base of parcels */
                i = 0;
                foreach (ShareParcel parcel in parcels)
                {
                    decimal costBaseReduction = apportionedAmounts[i++].Amount;

                    if (costBaseReduction <= parcel.CostBase)
                        ModifyParcel(unitOfWork, parcel, incomeReceived.PaymentDate, ParcelEvent.CostBaseReduction, parcel.Units, parcel.CostBase - costBaseReduction, "");
                    else
                    {
                        ModifyParcel(unitOfWork, parcel, incomeReceived.PaymentDate, ParcelEvent.CostBaseReduction, parcel.Units, 0.00m, "");

                        var cgtEvent = new CGTEvent(parcel.Stock, incomeReceived.TransactionDate, parcel.Units, parcel.CostBase, costBaseReduction - parcel.CostBase);
                        unitOfWork.CGTEventRepository.Add(cgtEvent);
                    }
                }
                CashAccount.AddTransaction(CashAccountTransactionType.Transfer, incomeReceived.PaymentDate, String.Format("Distribution for {0}", incomeReceived.ASXCode), incomeReceived.CashIncome);  
            }
            else
                CashAccount.AddTransaction(CashAccountTransactionType.Transfer, incomeReceived.PaymentDate, String.Format("Distribution for {0}", incomeReceived.ASXCode), incomeReceived.CashIncome); 
        }

        private void AddParcel(IPortfolioUnitOfWork unitOfWork, DateTime aquisitionDate, Guid stockId, int units, decimal unitPrice, decimal amount, decimal costBase, ParcelEvent parcelEvent)
        {
            var parcel = new ShareParcel(aquisitionDate, stockId, units, unitPrice, amount, costBase, parcelEvent);

            /* Handle Stapled Securities */
            var stock = _StockDatabase.StockQuery.Get(stockId, aquisitionDate);
            if (stock.Type == StockType.StapledSecurity)
            {
                /* Get child stocks */
                var childStocks = _StockDatabase.StockQuery.GetChildStocks(stockId, aquisitionDate);

                /* Apportion amount and cost base */
                ApportionedValue[] apportionedAmounts = new ApportionedValue[childStocks.Count];
                ApportionedValue[] apportionedCostBases = new ApportionedValue[childStocks.Count];
                ApportionedValue[] apportionedUnitPrices = new ApportionedValue[childStocks.Count];
                int i = 0;
                foreach (Stock childStock in childStocks)
                {
                    decimal percentageOfParent = childStock.PercentageOfParentCostBase(parcel.AquisitionDate);
                    int relativeValue = (int)(percentageOfParent * 10000);

                    apportionedAmounts[i].Units = relativeValue;
                    apportionedCostBases[i].Units = relativeValue;
                    apportionedUnitPrices[i].Units = relativeValue;
                    i++;
                }
                MathUtils.ApportionAmount(amount, apportionedAmounts);
                MathUtils.ApportionAmount(costBase, apportionedCostBases);
                MathUtils.ApportionAmount(unitPrice, apportionedUnitPrices);

                i = 0;
                foreach (Stock childStock in childStocks)
                {
                    var childParcel = new ShareParcel(aquisitionDate, childStock.Id, units, apportionedUnitPrices[i].Amount, apportionedAmounts[i].Amount, apportionedCostBases[i].Amount, parcelEvent);
                    childParcel.IncludeInHoldings = false;
                    childParcel.Parent = parcel.Id;

                    unitOfWork.ParcelRepository.Add(childParcel);

                    i++;
                }
                parcel.IncludeInParcels = false;
            }

            unitOfWork.ParcelRepository.Add(parcel);
        }

        private void ModifyParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime changeDate, ParcelEvent parcelEvent, int newUnits, decimal newCostBase, string description)
        {
            /* Update old effective dated record */
            parcel.ToDate = changeDate.AddDays(-1);
            unitOfWork.ParcelRepository.Update(parcel);

            /* Add new record */
            if (newUnits > 0)
            {
                var newParcel = parcel.Clone();
                newParcel.FromDate = changeDate;
                newParcel.ToDate = DateTimeConstants.NoEndDate();
                newParcel.Event = parcelEvent;
                newParcel.Units = newUnits;
                newParcel.CostBase = newCostBase;
                unitOfWork.ParcelRepository.Add(newParcel);
            }
        }

        private void DisposeOfParcel(IPortfolioUnitOfWork unitOfWork, ShareParcel parcel, DateTime disposalDate, int units, decimal amountReceived, string description)
        {
            decimal costBase;
            CGTEvent cgtEvent;

            /* Handle Stapled Securities */
            var stock = _StockDatabase.StockQuery.Get(parcel.Stock, disposalDate);
            if (stock.Type == StockType.StapledSecurity)
            {
                /* Get child parcels */
                var childParcels = _PortfolioDatabase.PortfolioQuery.GetChildParcels(parcel.Id);

                foreach (ShareParcel childParcel in childParcels)
                {
                    /* Modify Parcel */
                    costBase = childParcel.CostBase * ((decimal)units / childParcel.Units);
                    ModifyParcel(unitOfWork, childParcel, disposalDate, ParcelEvent.Disposal, childParcel.Units - units, childParcel.CostBase - costBase, description);

                    /* Create CGT Event */
                    var childStock = _StockDatabase.StockQuery.Get(childParcel.Stock, disposalDate);
                    decimal percentageOfParent = childStock.PercentageOfParentCostBase(disposalDate);
                    decimal childAmountReceived = amountReceived * percentageOfParent;
                    cgtEvent = new CGTEvent(childParcel.Stock, disposalDate, units, costBase, childAmountReceived);
                    unitOfWork.CGTEventRepository.Add(cgtEvent);
                }
            }

            /* Modify Parcel */
            costBase = parcel.CostBase * ((decimal)units / parcel.Units);
            ModifyParcel(unitOfWork, parcel, disposalDate, ParcelEvent.Disposal, parcel.Units - units, parcel.CostBase - costBase, description);

            /* Create CGT Event */
            if (stock.Type != StockType.StapledSecurity)
            {
                cgtEvent = new CGTEvent(parcel.Stock, disposalDate, units, costBase, amountReceived);
                unitOfWork.CGTEventRepository.Add(cgtEvent);
            }
        }

        public IReadOnlyCollection<ICorporateAction> GetUnappliedCorparateActions()
        {
            // Get a list of all stocks held
            var allOwnedStocks = _PortfolioDatabase.PortfolioQuery.GetStocksInPortfolio(this.Id);

            var allCorporateActions = new List<ICorporateAction>();
            foreach (OwnedStock ownedStock in allOwnedStocks)
            {
                var corporateActions = _StockDatabase.CorporateActionQuery.Find(ownedStock.Id, ownedStock.FromDate, ownedStock.ToDate);
                AddUnappliedCorporateActions(allCorporateActions, corporateActions);   
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

                if (corporateAction.Type == CorporateActionType.Dividend)
                {
                    Dividend dividend = corporateAction as Dividend;
                    date = dividend.ActionDate;
                    type = TransactionType.Income;
                    asxCode = _StockDatabase.StockQuery.Get(dividend.Stock, date).ASXCode;
                }
                else if (corporateAction.Type == CorporateActionType.CapitalReturn)
                {
                    CapitalReturn capitalReturn = corporateAction as CapitalReturn;
                    date = capitalReturn.PaymentDate;
                    type = TransactionType.ReturnOfCapital;
                    asxCode = _StockDatabase.StockQuery.Get(capitalReturn.Stock, date).ASXCode;
                }
                else if (corporateAction.Type == CorporateActionType.Transformation)
                {
                    Transformation transformation = corporateAction as Transformation;
                    date = transformation.ImplementationDate;
                    
                    if (transformation.ResultingStocks.Any())
                    {
                        type = TransactionType.OpeningBalance;
                        asxCode = _StockDatabase.StockQuery.Get(transformation.ResultingStocks.First().Stock, date).ASXCode;
                    }
                    else 
                    {
                        type = TransactionType.Disposal; 
                        asxCode = _StockDatabase.StockQuery.Get(transformation.Stock, date).ASXCode;
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


