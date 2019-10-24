﻿using System;
using System.Collections.Generic;
using System.Text;
using Booth.Common;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Transactions
{
    public class Aquisition : Transaction
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public override string Description
        {
            get { return "Aquired " + Units.ToString("n0") + " shares @ " + MathUtils.FormatCurrency(AveragePrice, false, true); }
        }
    }
}
