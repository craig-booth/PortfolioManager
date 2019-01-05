using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
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

        private HoldingCollection _Holdings;
        public IHoldingCollection Holdings => _Holdings;

        private TransactionService _Transactions;
        public ITransactionService Transactions => _Transactions;

        private CashAccount _CashAccount;
        public ICashAccount CashAccount => _CashAccount;

        private CgtEventCollection _CgtEvents;
        public ICgtEventCollection CgtEvents => _CgtEvents;

        public DateTime StartDate
        {
            get { return DateUtils.Earlist(_Transactions.Earliest, _CashAccount.Transactions.Earliest); }
        }

        public DateTime EndDate
        {
            get { return DateUtils.NoEndDate; }
        }

        public Portfolio(Guid id, string name, IEventStream eventStream)
        {
            Id = id;
            Name = name;
            _EventStream = eventStream;

            _Holdings = new HoldingCollection();
            _CashAccount = new CashAccount();
            _CgtEvents = new CgtEventCollection();
            _Transactions = new TransactionService(_Holdings, _CashAccount, _CgtEvents);           
        }

    }
}
