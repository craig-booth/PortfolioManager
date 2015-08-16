using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class ShareHolding
    {
        public Stock Stock { get; private set; }
        public int Units { get; private set; }
        public decimal AverageUnitPrice { get; private set; }
        public decimal Cost { get; private set; }
        public decimal UnitValue
        {
            get
            {
                return 10.00M; /* TODO: Priority Medium, get unit price at the holding date */
            }
        }
        public decimal MarketValue
        {
            get
            {
                return Units * UnitValue;
            }
        }

        private ShareHolding()
        {

        }

        public ShareHolding(Stock stock, int units, decimal averageUnitPrice, decimal cost)
        {
            Stock = stock;
            Units = units;
            AverageUnitPrice = averageUnitPrice;
            Cost = cost;
        }
    }
}
