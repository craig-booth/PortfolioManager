﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.Portfolios
{
    public class IncomeResponse
    {
        public class IncomeItem
        {
            public Stock Stock { get; set; }
            public decimal UnfrankedAmount { get; set; }
            public decimal FrankedAmount { get; set; }
            public decimal FrankingCredits { get; set; }
            public decimal NettIncome { get; set; }
            public decimal GrossIncome { get; set; }
        }

        public List<IncomeItem> Income { get; } = new List<IncomeItem>();
    }

}
