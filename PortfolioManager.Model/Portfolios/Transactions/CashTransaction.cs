using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Portfolios
{
    public class CashTransaction : Transaction
    {
        public decimal Amount { get; set; }

        protected override string GetDescription()
        {
            return "";
        }

        protected override TransactionType GetTransactionType()
        {
            return TransactionType.Deposit;
        }
    }
}
