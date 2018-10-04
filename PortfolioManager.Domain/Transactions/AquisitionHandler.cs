using System;
using System.Collections.Generic;
using System.Text;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class AquisitionHandler : ITransactionHandler
    {
        public void ApplyTransaction(Transaction transaction, Portfolio portfolio)
        {
            var aquisition = transaction as Aquisition;

            portfolio.Transactions.Add(aquisition);
        }

        
    }
}
