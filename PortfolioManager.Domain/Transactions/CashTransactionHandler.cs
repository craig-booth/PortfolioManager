using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class CashTransactionHandler : ITransactionHandler
    {
        public void ApplyTransaction(Transaction transaction, Portfolio portfolio)
        {
            var cashTransaction = transaction as CashTransaction;

            portfolio.Transactions.Add(cashTransaction);
        }
    }
}
