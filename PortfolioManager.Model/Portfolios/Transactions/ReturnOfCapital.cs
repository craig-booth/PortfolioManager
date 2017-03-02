using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Model.Portfolios
{

    public class ReturnOfCapital : Transaction
    {
        public decimal Amount { get; set; }
        public bool CreateCashTransaction { get; set; } 

        public ReturnOfCapital()
            : this (Guid.NewGuid())
        {

        }

        public ReturnOfCapital(Guid id)
            : base(id)
        {
            Type = TransactionType.ReturnOfCapital;
        }

        protected override string GetDescription()
        {
            return "Return of Capital of " + MathUtils.FormatCurrency(Amount, false, true);
        }
    }
}
