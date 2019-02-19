using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Portfolios.Events;
using PortfolioManager.Domain.Transactions;

namespace PortfolioManager.Domain.Portfolios
{
    public class Portfolio : ITrackedEntity
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; } = 0;
        private EventList _Events = new EventList();

        public string Name { get; private set; }

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

        public Portfolio()
        {
            _Holdings = new HoldingCollection();
            _CashAccount = new CashAccount();
            _CgtEvents = new CgtEventCollection();
            _Transactions = new TransactionService(_Holdings, _CashAccount, _CgtEvents);           
        }

        public void Create(Guid id, string name)
        {
            var @event = new PortfolioCreatedEvent(id, Version, name);
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(PortfolioCreatedEvent @event)
        {
            Version++;

            Id = @event.EntityId;
            Name = @event.Name;
        }

        protected void PublishEvent(Event @event)
        {
            _Events.Add(@event);
        }

        public IEnumerable<Event> FetchEvents()
        {
            return _Events.Fetch();
        }

        public void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;
                Apply(dynamicEvent);
            }
        }
    }
}
