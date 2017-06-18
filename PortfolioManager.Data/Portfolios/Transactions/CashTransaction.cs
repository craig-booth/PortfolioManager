using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
