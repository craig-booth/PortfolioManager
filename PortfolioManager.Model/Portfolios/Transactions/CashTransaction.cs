using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Portfolios
{
    public class CashTransaction : Transaction
    {
        public CashAccountTransactionType CashTransactionType { get; set; }
        public decimal Amount { get; set; }

        public CashTransaction()
            : this(Guid.NewGuid())
        {

        }

        public CashTransaction(Guid id)
            : base(id)
        {
        }

        protected override string GetDescription()
        {
            return "";
        }
    }
}
