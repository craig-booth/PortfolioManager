using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Models
{
    public class CostBaseAdjustmentTransaction : Transaction
    {
        public decimal Percentage { get; set; }
    }
}
