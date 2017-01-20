﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Utils;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Service
{
    public class ShareHoldingService
    {
        private readonly ParcelService _ParcelService;
        private readonly StockService _StockService;
        private readonly StockPriceService _StockPriceService;
        private readonly TransactionService _TransactionService;
        private readonly CashAccountService _CashAccountService;

        internal ShareHoldingService(ParcelService parcelService, StockService stockService, StockPriceService stockPriceService, TransactionService transactionService, CashAccountService cashAccountService)
        {
            _ParcelService = parcelService;
            _StockService = stockService;
            _StockPriceService = stockPriceService;
            _TransactionService = transactionService;
            _CashAccountService = cashAccountService;
        }

        public ShareHolding GetHolding(Stock stock, DateTime date)
        {
            IReadOnlyCollection<ShareParcel> parcels;

            if (stock.Type == StockType.StapledSecurity)
                parcels = _ParcelService.GetStapledSecurityParcels(stock, date);
            else
                parcels = _ParcelService.GetParcels(stock, date);

            var holding = new ShareHolding();
            holding.Stock = stock;

            foreach (var parcel in parcels)
            {
                holding.Units += parcel.Units;
                holding.TotalCostBase += parcel.CostBase;
                holding.TotalCost += parcel.Units * parcel.UnitPrice;

            }

            return holding;

        }

        public IReadOnlyCollection<ShareHolding> GetHoldings(DateTime date)
        { 
            var allParcels = _ParcelService.GetParcels(date).OrderBy(x => x.Stock);

            var holdings = new List<ShareHolding>();
            ShareHolding holding = null;
            foreach (var parcel in allParcels)
            {
                if (parcel.Units <= 0)
                    continue;

                if ((holding == null) || (parcel.Stock != holding.Stock.Id))
                {
                    var stock = _StockService.Get(parcel.Stock, date);

                    // If a stapled security then get the parent stock
                    if (stock.ParentId != Guid.Empty)
                    {
                        holding = holdings.FirstOrDefault(x => x.Stock.Id == stock.ParentId);
                        if (holding != null)
                        {
                            holding.TotalCostBase += parcel.CostBase;
                            holding.TotalCost += parcel.Units * parcel.UnitPrice;
                        }
                        else
                        {
                            stock = _StockService.Get(stock.ParentId, date);

                            holding = new ShareHolding();
                            holdings.Add(holding);

                            holding.Stock = stock;
                            holding.Units = parcel.Units;
                            holding.TotalCostBase = parcel.CostBase;
                            holding.TotalCost = parcel.Units * parcel.UnitPrice;
                        }

                    }
                    else
                    {
                        holding = new ShareHolding();
                        holdings.Add(holding);

                        holding.Stock = stock;
                        holding.Units = parcel.Units;
                        holding.TotalCostBase = parcel.CostBase;
                        holding.TotalCost = parcel.Units * parcel.UnitPrice;
   
                    }
                }
                else
                {
                    holding.Units += parcel.Units;
                    holding.TotalCostBase += parcel.CostBase;
                    holding.TotalCost += parcel.Units * parcel.UnitPrice;
                }

            }

            /* Add prices to the Holdings list */
            decimal stockPrice;
            foreach (var shareHolding in holdings)
            {
                /* Retrieve the stock price information */
                if (date == DateTime.Today)
                {
                    stockPrice = _StockPriceService.GetCurrentPrice(shareHolding.Stock);
                }
                else
                {
                    if (!_StockPriceService.TryGetClosingPrice(shareHolding.Stock, date, out stockPrice))
                    {
                        // If no price available then use average cost
                        stockPrice = shareHolding.AverageUnitCost;
                    }
                }

                shareHolding.UnitValue = stockPrice;
            }

            return holdings.AsReadOnly();

        }

        public DateTime GetPortfolioStartDate()
        {
            DateTime parcelStartDate;
            DateTime cashStartDate;

            var firstParcel = _ParcelService.GetParcels(DateUtils.NoStartDate, DateTime.Today).OrderBy(x => x.FromDate).FirstOrDefault();         
            if (firstParcel != null)
                parcelStartDate = firstParcel.FromDate;
            else
                parcelStartDate = DateTime.Today;

            var firstCashTransaction = _CashAccountService.GetTransactions(DateUtils.NoStartDate, DateTime.Today).OrderBy(x => x.Date).FirstOrDefault();
            if (firstCashTransaction != null)
                cashStartDate = firstCashTransaction.Date;
            else
                cashStartDate = DateTime.Today;

            if (parcelStartDate <= cashStartDate)
                return parcelStartDate;
            else
                return cashStartDate;
        }

        public IReadOnlyCollection<Stock> GetOwnedStocks(DateTime date, bool includeChildStocks)
        {
            return GetOwnedStocks(date, date, includeChildStocks);
        }

        public IReadOnlyCollection<Stock> GetOwnedStocks(DateTime fromDate, DateTime toDate, bool includeChildStocks)
        {
            var ownedStocks = new List<Stock>();

            var parcels = _ParcelService.GetParcels(fromDate, toDate).OrderBy(x => x.Stock);

            Stock currentStock = null;
            foreach (var shareParcel in parcels)
            {
                if ((currentStock == null) || (shareParcel.Stock != currentStock.Id))
                {
                    var stock = _StockService.Get(shareParcel.Stock, shareParcel.FromDate);
                    currentStock = stock;

                    if (!includeChildStocks)
                    {
                        if (stock.ParentId != Guid.Empty)
                            stock = _StockService.Get(stock.ParentId, shareParcel.FromDate);
                    }

                    if (ownedStocks.FindLast(x => x.Id == stock.Id) == null)
                        ownedStocks.Add(stock);           
                }
            }

            return ownedStocks.AsReadOnly();
        }

        public IReadOnlyCollection<OwnedStockId> GetOwnedStockIds(DateTime date)
        {
            var ownedStocks = new List<OwnedStockId>();

            var parcels = _ParcelService.GetParcels(date);

            foreach (var shareParcel in parcels)
            {
                // Make sure that we get the oldest effectve record for this parcel
                var firstDate = _ParcelService.GetParcels(shareParcel.Id, DateUtils.NoStartDate, date).Min(x => x.FromDate);

                var ownedStock = new OwnedStockId()
                {
                    Id = shareParcel.Stock,
                    FromDate = firstDate,
                    ToDate = shareParcel.ToDate
                };
                InsertOverlappedStockId(ownedStocks, ownedStock);

            }

            return ownedStocks.AsReadOnly();
        }

        private void InsertOverlappedStockId(List<OwnedStockId> list, OwnedStockId entry)
        {
            var overlappedEntries = list.Where(x => ((x.Id == entry.Id) &&
                                                     (((x.FromDate <= entry.FromDate) && (x.ToDate >= entry.FromDate)) ||
                                                       (x.FromDate <= entry.ToDate) && (x.ToDate >= entry.FromDate)))).ToList();           

            foreach (var overlappedEntry in overlappedEntries)
            {
                if (overlappedEntry.FromDate < entry.FromDate)
                    entry.FromDate = overlappedEntry.FromDate;

                if (overlappedEntry.ToDate > entry.ToDate)
                    entry.ToDate = overlappedEntry.ToDate;

                list.Remove(overlappedEntry);
            }

            list.Add(entry);
        }

        public decimal CalculateIRR(DateTime startDate, DateTime endDate)
        {
            var cashFlows = new CashFlows();

            // Get the initial portfolio value
            var initialHoldings = GetHoldings(startDate);
            var initialHoldingsValue = initialHoldings.Sum(x => x.MarketValue);

            // Get initial Cash Account Balance
            var initialCashBalance = _CashAccountService.GetBalance(startDate);

            // Add the initial portfolio value
            var initialValue = initialHoldingsValue + initialCashBalance;
            cashFlows.Add(startDate, -initialValue);
                     
            // generate list of cashFlows
            var transactions = _TransactionService.GetTransactions(startDate.AddDays(1), endDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Type == TransactionType.CashTransaction)
                {
                    var cashTransaction = transaction as CashTransaction;
                    if ((cashTransaction.CashTransactionType == CashAccountTransactionType.Deposit) ||
                        (cashTransaction.CashTransactionType == CashAccountTransactionType.Withdrawl))
                        cashFlows.Add(cashTransaction.TransactionDate, -cashTransaction.Amount);
                }
            }
            
            // Get the final portfolio value
            var finalHoldings = GetHoldings(endDate);
            var finalHoldingsValue = finalHoldings.Sum(x => x.MarketValue);

            // Get final Cash Account Balance
            var finalCashBalance = _CashAccountService.GetBalance(endDate);

            // Add the final portfolio value
            var finalValue = finalHoldingsValue + finalCashBalance;
            cashFlows.Add(endDate, finalValue);

            var irr = IRRCalculator.CalculateIRR(cashFlows);

            return (decimal)Math.Round(irr, 5);
        }
    }

    public class OwnedStockId
    {
        public Guid Id;
        public DateTime FromDate;
        public DateTime ToDate;
    }

}
