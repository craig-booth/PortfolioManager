using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Web.Mapping
{
    public class TransactionHandler
    {
        private Dictionary<Type, ITransactionHandler> _Handlers = new Dictionary<Type, ITransactionHandler>();

        public TransactionHandler(TransactionConfiguration configuration)
        {
            foreach (var item in configuration.Items)
                _Handlers.Add(item.DomainTransactionType, item.Handler);
        }

        public void Handle(Domain.Transactions.Transaction transaction, Portfolio portfolio)
        {
            if (_Handlers.TryGetValue(transaction.GetType(), out var handler))
                handler.ApplyTransaction(transaction, portfolio);
        }
    }
}
