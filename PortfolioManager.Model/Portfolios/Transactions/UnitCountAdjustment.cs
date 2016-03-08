using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    public class UnitCountAdjustment : Transaction 
    {
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
        public string Comment { get; set; }
        
        public UnitCountAdjustment()
            : base(Guid.NewGuid())
        {

        }

        public UnitCountAdjustment(Guid id)
            : base(id)
        {

        }

        protected override string GetDescription()
        {
            return "Adjust unit count using ratio " + OriginalUnits.ToString("n0") + ":" + NewUnits.ToString("n0");
        }

        protected override TransactionType GetTransactionType()
        {
            return TransactionType.UnitCountAdjustment;
        }


    }
}
