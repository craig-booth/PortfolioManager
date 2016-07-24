using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{

    public class ReturnOfCapital : Transaction
    {
        public decimal Amount { get; set; }

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
