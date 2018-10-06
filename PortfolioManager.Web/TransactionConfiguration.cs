using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Domain.Transactions;

namespace PortfolioManager.Web
{
    public class TransactionConfiguration
    {

        private List<TransactionConfigurationItem> _Items = new List<TransactionConfigurationItem>();
        public IEnumerable<TransactionConfigurationItem> Items
        {
            get { return _Items; }
        }

        public void RegisterTransaction<D, R>(string name, ITransactionHandler handler)
            where D : Domain.Transactions.Transaction
            where R : RestApi.Transactions.Transaction
        {
            _Items.Add(new TransactionConfigurationItem(name, typeof(D), typeof(R), handler));
        }

        public class TransactionConfigurationItem
        {
            public string Name;
            public Type DomainTransactionType;
            public Type RestApiTransactionType;
            public ITransactionHandler Handler;

            public TransactionConfigurationItem(string name, Type domainType, Type restApiType, ITransactionHandler handler)
            {
                Name = name;
                DomainTransactionType = domainType;
                RestApiTransactionType = restApiType;
                Handler = handler;
            }
        }
    }
}
