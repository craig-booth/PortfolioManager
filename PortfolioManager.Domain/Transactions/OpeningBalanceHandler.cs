using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class OpeningBalanceHandler : ITransactionHandler
    {
        public void ApplyTransaction(Transaction transaction, Portfolio portfolio)
        {
            var openingBalance = transaction as OpeningBalance;

            var holding = portfolio.Holdings.Get(openingBalance.Stock.Id);
            if (holding == null)
            {
                holding = portfolio.Holdings.Add(openingBalance.Stock, openingBalance.TransactionDate);
            }

            holding.AddParcel(openingBalance.TransactionDate, openingBalance.AquisitionDate, openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase);

            portfolio.Transactions.Add(openingBalance); 
        }
    }
}
