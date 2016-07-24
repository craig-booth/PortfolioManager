using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class OpeningBalance : Transaction
    {
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public OpeningBalance()
            : this(Guid.NewGuid())
        {

        }

        public OpeningBalance(Guid id)
            : base(id)
        {
            Type = TransactionType.OpeningBalance;
        }

        protected override string GetDescription()
        {
            return "Opening balance of " + Units.ToString("n0") + " shares";
        }
    }
}
