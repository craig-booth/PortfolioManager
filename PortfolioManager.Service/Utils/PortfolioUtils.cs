﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Utils
{
    static class PortfolioUtils
    { 
        public static ApportionedCurrencyValue[] ApportionAmountOverParcels(IEnumerable<ShareParcel> parcels, decimal amount)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[parcels.Count()];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }

        public static ApportionedIntegerValue[] ApportionAmountOverParcels(IEnumerable<ShareParcel> parcels, int amount)
        {
            ApportionedIntegerValue[] result = new ApportionedIntegerValue[parcels.Count()];
            int i = 0;
            foreach (ShareParcel parcel in parcels)
                result[i++].Units = parcel.Units;
            MathUtils.ApportionAmount(amount, result);

            return result;
        }
        /*
        public static ApportionedCurrencyValue[] ApportionAmountOverChildStocks(IEnumerable<Data.Stocks.Stock> childStocks, DateTime atDate, decimal amount, IStockQuery stockQuery)
        {
            ApportionedCurrencyValue[] result = new ApportionedCurrencyValue[childStocks.Count()];
            int i = 0;
            foreach (var childStock in childStocks)
            {
                decimal percentageOfParent = stockQuery.PercentOfParentCost(childStock.ParentId, childStock.Id, atDate);
                int relativeValue = (int)(percentageOfParent * 10000);

                result[i].Units = relativeValue;
                i++;
            }
            MathUtils.ApportionAmount(amount, result);

            return result;
        } */

      /*  public static IReadOnlyCollection<ShareParcel> GetStapledSecurityParcels(Data.Stocks.Stock stock, DateTime date, IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            var stapledParcels = new List<ShareParcel>();

            var childStocks = stockQuery.GetChildStocks(stock.Id, date);

            foreach (var childStock in childStocks)
            {
                var childParcels = portfolioQuery.GetParcelsForStock(childStock.Id, date, date);

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
        } */


        public static DateTime GetPortfolioStartDate(IPortfolioQuery portfolioQuery)
        {
            DateTime parcelStartDate;
            DateTime cashStartDate;

            var firstParcel = portfolioQuery.GetAllParcels(DateUtils.NoStartDate, DateTime.Today).OrderBy(x => x.FromDate).FirstOrDefault();
            if (firstParcel != null)
                parcelStartDate = firstParcel.FromDate;
            else
                parcelStartDate = DateTime.Today;

            var firstCashTransaction = portfolioQuery.GetCashAccountTransactions(DateUtils.NoStartDate, DateTime.Today).OrderBy(x => x.Date).FirstOrDefault();
            if (firstCashTransaction != null)
                cashStartDate = firstCashTransaction.Date;
            else
                cashStartDate = DateTime.Today;

            if (parcelStartDate <= cashStartDate)
                return parcelStartDate;
            else
                return cashStartDate;
        }

        public static HoldingItem GetHolding(Guid stockId, DateTime date, IPortfolioQuery portfolioQuery, StockExchange stockExchange)
        {
            IEnumerable<ShareParcel> parcels;

            var stock = stockExchange.Stocks.Get(stockId);

            /*    if (stock.Type == StockType.StapledSecurity)
                    parcels = GetStapledSecurityParcels(stock, date, portfolioQuery, stockQuery);
                else */
            parcels = portfolioQuery.GetParcelsForStock(stock.Id, date, date);

            var holding = new HoldingItem();
            holding.Stock = stock.ToStockItem(date);
            holding.Category = stock.Properties[date].Category;

            foreach (var parcel in parcels)
            {
                holding.Units += parcel.Units;
                holding.Cost += parcel.Units * parcel.UnitPrice;
            }
            holding.Value = holding.Units * stock.GetPrice(date);

            return holding;
        }

        public static IEnumerable<HoldingItem> GetHoldings(DateTime date, IPortfolioQuery portfolioQuery, StockExchange stockExchange)
        {
            var holdings = new List<HoldingItem>();

            var holdingQuery = from parcel in portfolioQuery.GetAllParcels(date, date)
                               group parcel by parcel.Stock into parcelGroup
                               select parcelGroup;

            foreach (var parcelGroup in holdingQuery)
            {
                var stock = stockExchange.Stocks.Get(parcelGroup.Key);

                var holding = new HoldingItem();
                holding.Stock = stock.ToStockItem(date);
                holding.Category = stock.Properties[date].Category;

                foreach (var parcel in parcelGroup)
                {
                    holding.Units += parcel.Units;
                    holding.Cost += parcel.Units * parcel.UnitPrice;
                }
                holding.Value = holding.Units * stock.GetPrice(date);

                holdings.Add(holding);
            }

            return holdings;
        }

        public static IEnumerable<HoldingItem> GetTradeableHoldings(DateTime date, IPortfolioQuery portfolioQuery, StockExchange stockExchange)
        {
            var holdings = new List<HoldingItem>();

            var holdingQuery = from parcel in portfolioQuery.GetAllParcels(date, date)
                               group parcel by parcel.Stock into parcelGroup
                               select parcelGroup;

            foreach (var parcelGroup in holdingQuery)
            {
                HoldingItem holding;

                var stock = stockExchange.Stocks.Get(parcelGroup.Key);

                holding = new HoldingItem();
                holding.Stock = stock.ToStockItem(date);
                foreach (var parcel in parcelGroup)
                {
                    holding.Units += parcel.Units;
                    holding.Cost += parcel.Units * parcel.UnitPrice;
                }
                holding.Value = holding.Units * stock.GetPrice(date);

                holdings.Add(holding);
            }

            return holdings;
        }

        public static decimal CalculatePortfolioIRR(DateTime startDate, DateTime endDate, IPortfolioQuery portfolioQuery, StockExchange stockExchange)
        {
            var cashFlows = new CashFlows();

            // Get the initial portfolio value
            var initialHoldings = GetHoldings(startDate, portfolioQuery, stockExchange);
            var initialHoldingsValue = initialHoldings.Sum(x => x.Value);

            // Get initial Cash Account Balance
            var initialCashBalance = portfolioQuery.GetCashBalance(startDate);

            // Add the initial portfolio value
            var initialValue = initialHoldingsValue + initialCashBalance;
            cashFlows.Add(startDate, -initialValue);

            // generate list of cashFlows
            var transactions = portfolioQuery.GetTransactions(startDate.AddDays(1), endDate);
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
            var finalHoldings = GetHoldings(endDate, portfolioQuery, stockExchange);
            var finalHoldingsValue = finalHoldings.Sum(x => x.Value);

            // Get final Cash Account Balance
            var finalCashBalance = portfolioQuery.GetCashBalance(endDate);

            // Add the final portfolio value
            var finalValue = finalHoldingsValue + finalCashBalance;
            cashFlows.Add(endDate, finalValue);

            var irr = IRRCalculator.CalculateIRR(cashFlows);

            return (decimal)Math.Round(irr, 5);
        }
    }


}
