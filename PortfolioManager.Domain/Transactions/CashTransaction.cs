using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.Domain.Transactions
{
    public class CashTransaction :Transaction
    {
        public BankAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }

        public override string Description
        {
            get { return Comment; }
        }
    }
}
