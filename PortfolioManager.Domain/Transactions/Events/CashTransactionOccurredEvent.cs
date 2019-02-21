using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Transactions.Events
{
    public class CashTransactionOccurredEvent : TransactionOccurredEvent
    {
        public BankAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransactionOccurredEvent(Guid entityId, int version, Guid transactionId, DateTime date, Guid stock, string comment)
            : base(entityId, version, transactionId, date, stock, comment)
        {

        }

    }
}
