using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{

    public class CashAccountTransaction  : Entity
    {
        public BankAccountTransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public CashAccountTransaction()
            : this(Guid.NewGuid())
        {
        }

        public CashAccountTransaction(Guid id)
            :base(id)
        {

        }
    }
}