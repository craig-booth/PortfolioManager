using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Service
{
    public class ShareHolding
    {
        public Stock Stock { get; set; }
        public int Units { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostBase { get; set; }
        public decimal UnitValue { get; set; }

        public decimal AverageUnitCost
        {
            get
            {
                if (Units > 0)
                    return TotalCost / Units;
                else
                    return 0;
            }
        }

        public decimal MarketValue
        {
            get
            {
                return Units * UnitValue;
            }
        }
        
    }
}
