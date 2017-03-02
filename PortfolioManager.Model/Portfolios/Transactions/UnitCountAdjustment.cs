using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PortfolioManager.Model.Portfolios
{
    public class UnitCountAdjustment : Transaction 
    {
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
        
        public UnitCountAdjustment()
            : this(Guid.NewGuid())
        {

        }

        public UnitCountAdjustment(Guid id)
            : base(id)
        {
            Type = TransactionType.UnitCountAdjustment;
        }

        protected override string GetDescription()
        {
            return "Adjust unit count using ratio " + OriginalUnits.ToString("n0") + ":" + NewUnits.ToString("n0");
        }
    }
}
