using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.Portfolios.Events
{
    public class CashTransactionOccurredEvent : TransactionnOccurredEvent
    {
        public BankAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransactionOccurredEvent(Guid entityId, int version, DateTime date, Guid stock, string comment)
            : base(entityId, version, date, stock, comment)
        {

        }

    }
}
