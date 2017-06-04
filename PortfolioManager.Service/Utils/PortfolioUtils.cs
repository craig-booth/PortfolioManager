using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Utils
{
    class PortfolioUtils
    {
        private IPortfolioQuery _PortfolioQuery;
        private IStockQuery _StockQuery;
        private StockUtils _StockUtils;

        public PortfolioUtils(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _PortfolioQuery = portfolioQuery;
            _StockQuery = stockQuery;
            _StockUtils = new StockUtils(stockQuery);
        }

        public static ApportionedCurrencyValue[] ApportionAmountOverParcels(IReadOnlyCollection<ShareParcel> parcels, decimal amount)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[parcels.Count];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public static ApportionedIntegerValue[] ApportionAmountOverParcels(IReadOnlyCollection<ShareParcel> parcels, int amount)
        {
            ApportionedIntegerValue[] result = new ApportionedIntegerValue[parcels.Count];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public ApportionedCurrencyValue[] ApportionAmountOverChildStocks(IReadOnlyCollection<Stock> childStocks, DateTime atDate, decimal amount)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[childStocks.Count];
            int i = 0;
            foreach (Stock childStock in childStocks)
            {
                decimal percentageOfParent = _StockQuery.PercentOfParentCost(childStock.ParentId, childStock.Id, atDate);
                int relativeValue = (int)(percentageOfParent * 10000);

                result[i].Units = relativeValue;
                i++;
            }
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public IReadOnlyCollection<ShareParcel> GetStapledSecurityParcels(Stock stock, DateTime date)
        {
            var stapledParcels = new List<ShareParcel>();

            var childStocks = _StockQuery.GetChildStocks(stock.Id, date);

            foreach (var childStock in childStocks)
            {
                var childParcels = _PortfolioQuery.GetParcelsForStock(childStock.Id, date, date);

                foreach (var childParcel in childParcels)
                {
                    var stapledParcel = stapledParcels.FirstOrDefault(x => x.PurchaseId == childParcel.PurchaseId);
                    if (stapledParcel == null)
                    {
                        stapledParcel = new ShareParcel(childParcel.AquisitionDate, stock.Id, childParcel.Units, childParcel.UnitPrice, childParcel.Amount, childParcel.CostBase, childParcel.PurchaseId);
                        stapledParcels.Add(stapledParcel);
                    }
                    else
                    {
                        stapledParcel.Amount += childParcel.Amount;
                        stapledParcel.CostBase += childParcel.CostBase;
                        stapledParcel.UnitPrice += childParcel.UnitPrice;
                    }

                }
            }

            return stapledParcels;
        }


        public DateTime GetPortfolioStartDate()
        {
            DateTime parcelStartDate;
            DateTime cashStartDate;

            var firstParcel = _PortfolioQuery.GetAllParcels(DateUtils.NoStartDate, DateTime.Today).OrderBy(x => x.FromDate).FirstOrDefault();
            if (firstParcel != null)
                parcelStartDate = firstParcel.FromDate;
            else
                parcelStartDate = DateTime.Today;

            var firstCashTransaction = _PortfolioQuery.GetCashAccountTransactions(DateUtils.NoStartDate, DateTime.Today).OrderBy(x => x.Date).FirstOrDefault();
            if (firstCashTransaction != null)
                cashStartDate = firstCashTransaction.Date;
            else
                cashStartDate = DateTime.Today;

            if (parcelStartDate <= cashStartDate)
                return parcelStartDate;
            else
                return cashStartDate;
        }

        public HoldingItem GetHolding(Guid stockId, DateTime date)
        {
            IReadOnlyCollection<ShareParcel> parcels;

            var stock = _StockQuery.Get(stockId, date);

            if (stock.Type == StockType.StapledSecurity)
                parcels = GetStapledSecurityParcels(stock, date);
            else
                parcels = _PortfolioQuery.GetParcelsForStock(stock.Id, date, date);

            var holding = new HoldingItem();
            holding.Stock = new StockItem(stock);
            holding.Category = stock.Category; 

            foreach (var parcel in parcels)
            {
                holding.Units += parcel.Units;
                holding.Cost += parcel.Units * parcel.UnitPrice;
            }
            holding.Value = holding.Units * _StockUtils.GetPrice(stock.Id, date);

            return holding;
        }

        public IEnumerable<HoldingItem> GetHoldings(DateTime date)
        {
            var holdings = new List<HoldingItem>();

            var holdingQuery = from parcel in _PortfolioQuery.GetAllParcels(date, date)
                                group parcel by parcel.Stock into parcelGroup
                                select parcelGroup;

            foreach (var parcelGroup in holdingQuery)
            {
                var holding = new HoldingItem();
                holding.Stock = _StockUtils.Get(parcelGroup.Key, date);
                foreach (var parcel in parcelGroup)
                {
                    holding.Units += parcel.Units;
                    holding.Cost += parcel.Units * parcel.UnitPrice;
                }
                holding.Value = holding.Units * _StockUtils.GetPrice(parcelGroup.Key, date);

                holdings.Add(holding);
            }
            
            return holdings;
        }

        public IEnumerable<HoldingItem> GetTradeableHoldings(DateTime date)
        {
            var holdings = new List<HoldingItem>();

            var holdingQuery = from parcel in _PortfolioQuery.GetAllParcels(date, date)
                               group parcel by parcel.Stock into parcelGroup
                               select parcelGroup;

            foreach (var parcelGroup in holdingQuery)
            {
                HoldingItem holding;

                var stock = _StockQuery.Get(parcelGroup.Key, date);

                if (stock.ParentId != Guid.Empty)
                {
                    stock = _StockQuery.Get(stock.ParentId, date);

                    // Check if the parent stock has already been added
                    holding = holdings.FirstOrDefault(x => x.Stock.Id == stock.Id);
                    if (holding != null)
                    {
                        foreach (var parcel in parcelGroup)
                        {
                            holding.Cost += parcel.Units * parcel.UnitPrice;
                        }
                        continue;
                    }

                }

                holding = new HoldingItem();
                holding.Stock = new StockItem(stock);
                foreach (var parcel in parcelGroup)
                {
                    holding.Units += parcel.Units;
                    holding.Cost += parcel.Units * parcel.UnitPrice;
                }
                holding.Value = holding.Units * _StockUtils.GetPrice(stock.Id, date);

                holdings.Add(holding);
            }

            return holdings;
        }

        public decimal CalculatePortfolioIRR(DateTime startDate, DateTime endDate)
        {
            var cashFlows = new CashFlows();

            // Get the initial portfolio value
            var initialHoldings = GetHoldings(startDate);
            var initialHoldingsValue = initialHoldings.Sum(x => x.Value);

            // Get initial Cash Account Balance
            var initialCashBalance = _PortfolioQuery.GetCashBalance(startDate);

            // Add the initial portfolio value
            var initialValue = initialHoldingsValue + initialCashBalance;
            cashFlows.Add(startDate, -initialValue);

            // generate list of cashFlows
            var transactions = _PortfolioQuery.GetTransactions(startDate.AddDays(1), endDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Type == TransactionType.CashTransaction)
                {
                    var cashTransaction = transaction as CashTransaction;
                    if ((cashTransaction.CashTransactionType == BankAccountTransactionType.Deposit) ||
                        (cashTransaction.CashTransactionType == BankAccountTransactionType.Withdrawl))
                        cashFlows.Add(cashTransaction.TransactionDate, -cashTransaction.Amount);
                }
            }

            // Get the final portfolio value
            var finalHoldings = GetHoldings(endDate);
            var finalHoldingsValue = finalHoldings.Sum(x => x.Value);

            // Get final Cash Account Balance
            var finalCashBalance = _PortfolioQuery.GetCashBalance(endDate);

            // Add the final portfolio value
            var finalValue = finalHoldingsValue + finalCashBalance;
            cashFlows.Add(endDate, finalValue);

            var irr = IRRCalculator.CalculateIRR(cashFlows);

            return (decimal)Math.Round(irr, 5);
        }

    }


}
