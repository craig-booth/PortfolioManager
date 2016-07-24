using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class CostBaseAdjustment : Transaction
    {
        public decimal Percentage { get; set; }

        public CostBaseAdjustment()
            : this(Guid.NewGuid())
        {

        }

        public CostBaseAdjustment(Guid id)
            : base(id)
        {
            Type = TransactionType.CostBaseAdjustment;
        }

        protected override string GetDescription()
        {
            return "Adjust cost base by " + Percentage.ToString("P");
        }

    }
}
