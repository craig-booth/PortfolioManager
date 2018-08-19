using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.RestApi.Stocks;

namespace PortfolioManager.RestApi.Portfolio
{
    public class HoldingResponse
    {
        public StockResponse Stock { get; set; }

        public int Units;
        public decimal Amount;
        public decimal CostBase;
    }
}
