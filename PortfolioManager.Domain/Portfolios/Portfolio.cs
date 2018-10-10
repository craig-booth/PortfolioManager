using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;
using PortfolioManager.Domain.Transactions;


namespace PortfolioManager.Domain.Portfolios
{
    public class Portfolio
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public int Version { get; private set; } = 0;
        protected IEventStream _EventStream;

        public IHoldingCollection Holdings { get; } = new HoldingCollection();
        public ITransactionCollection Transactions { get; } = new TransactionCollection();

        public CashAccount CashAccount { get; } = new CashAccount(); 

        public Portfolio(Guid id, string name, IEventStream eventStream)
        {
            Id = id;
            Name = name;
            _EventStream = eventStream;
        }

    }
}
