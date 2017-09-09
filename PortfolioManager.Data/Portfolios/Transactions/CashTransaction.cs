using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{
    public class CashTransaction : Transaction
    {
        public BankAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransaction()
            : this(Guid.NewGuid())
        {
            ASXCode = "";
            Type = TransactionType.CashTransaction;
        }

        public CashTransaction(Guid id)
            : base(id)
        {
            ASXCode = "";
            Type = TransactionType.CashTransaction;
        }

        protected override string GetDescription()
        {
            return Comment;
        }
    }
}
