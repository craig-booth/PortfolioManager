using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{
    public class UnitCountAdjustment : Transaction 
    {
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
        
        public UnitCountAdjustment()
            : this(Guid.NewGuid())
        {
            Type = TransactionType.UnitCountAdjustment;
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
