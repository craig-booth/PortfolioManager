using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Models
{
    public class UnitCountAdjustmentTransaction: Transaction
    {
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
    }
}
