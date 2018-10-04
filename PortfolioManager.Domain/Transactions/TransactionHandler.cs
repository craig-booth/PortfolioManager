using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public interface ITransactionHandler
    {
        void ApplyTransaction(Transaction transaction, Portfolio portfolio);
    }

}