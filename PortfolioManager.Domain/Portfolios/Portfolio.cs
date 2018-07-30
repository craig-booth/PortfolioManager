using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Portfolios
{
    public class Portfolio
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public int Version { get; private set; } = 0;
        protected IEventStream _EventStream;

        private Dictionary<Guid, Holding> _Holdings;

        public CashAccount CashAccount { get; private set; } 

        public Portfolio(Guid id, string name, IEventStream eventStream)
        {
            Id = id;
            Name = name;
            _EventStream = eventStream;

            _Holdings = new Dictionary<Guid, Holding>();

            CashAccount = new CashAccount();
        }

        public Holding GetHolding(Stock stock)
        {
            if (_Holdings.ContainsKey(stock.Id))
                return _Holdings[stock.Id];
            else
                return null;
        }

        public void PurchaseStock(Stock stock, DateTime purchaseDate, int units, decimal averagePrice, decimal transactionCosts)
        {
            if (! _Holdings.TryGetValue(stock.Id, out var holding))
            {
                holding = new Holding(stock, purchaseDate);
                _Holdings.Add(stock.Id, holding);
            }

            decimal amountPaid = (units * averagePrice) + transactionCosts;
            decimal costBase = amountPaid;

            holding.AddParcel(purchaseDate, purchaseDate, units, averagePrice, amountPaid, costBase);
        }
    }


}
