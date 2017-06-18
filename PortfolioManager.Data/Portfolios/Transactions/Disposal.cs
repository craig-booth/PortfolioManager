using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{

    public class Disposal : Transaction
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public bool CreateCashTransaction { get; set; }

        public Disposal()
            : this(Guid.NewGuid())
        {

        }

        public Disposal(Guid id)
            : base(id)
        {
            Type = TransactionType.Disposal;
        }

        protected override string GetDescription()
        {
            return "Disposed of " + Units.ToString("n0") + " shares @ " + MathUtils.FormatCurrency(AveragePrice, false, true);
        }
    }
}
