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

        public CashTransaction()
            : this(Guid.NewGuid())
        {

        }

        public CashTransaction(Guid id)
            : base(id)
        {
            Type = TransactionType.Deposit;
        }

        protected override string GetDescription()
        {
            return "";
        }
    }
}
