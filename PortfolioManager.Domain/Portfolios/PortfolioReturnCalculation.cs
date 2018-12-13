﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Utils;
using PortfolioManager.Domain.Transactions;

namespace PortfolioManager.Domain.Portfolios
{
    public static class PortfolioReturnCalculation
    {
        public static decimal CalculateIRR(this Portfolio portfolio, DateRange dateRange)
        {
            var cashFlows = new CashFlows();

            // Get the initial portfolio value         
            var initialHoldings = portfolio.Holdings.All(dateRange.FromDate);
            var initialHoldingsValue = initialHoldings.Sum(x => x.Properties[dateRange.FromDate].Amount);

            // Get initial Cash Account Balance
            var initialCashBalance = portfolio.CashAccount.Balance(dateRange.FromDate);

            // Add the initial portfolio value
            var initialValue = initialHoldingsValue + initialCashBalance;
            cashFlows.Add(dateRange.FromDate, -initialValue);

            // generate list of cashFlows
            var transactions = portfolio.Transactions.InDateRange(dateRange);
            foreach (var transaction in transactions)
            {
                if (transaction is CashTransaction)
                {
                    var cashTransaction = transaction as CashTransaction;
                    if ((cashTransaction.CashTransactionType == BankAccountTransactionType.Deposit) ||
                        (cashTransaction.CashTransactionType == BankAccountTransactionType.Withdrawl))
                        cashFlows.Add(cashTransaction.TransactionDate, -cashTransaction.Amount);
                }
            }

            // Get the final portfolio value
            var finalHoldings = portfolio.Holdings.All(dateRange.ToDate);
            var finalHoldingsValue = initialHoldings.Sum(x => x.Properties[dateRange.ToDate].Amount);

            // Get final Cash Account Balance
            var finalCashBalance = portfolio.CashAccount.Balance(dateRange.ToDate);

            // Add the final portfolio value
            var finalValue = finalHoldingsValue + finalCashBalance;
            cashFlows.Add(dateRange.ToDate, finalValue);

            var irr = IrrCalculator.CalculateIRR(cashFlows);

            return (decimal)Math.Round(irr, 5);
        }
    }
}
