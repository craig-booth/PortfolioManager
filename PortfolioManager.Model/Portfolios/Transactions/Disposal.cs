﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{

    public enum CGTCalculationMethod { MinimizeGain, MaximizeGain, FirstInFirstOut, LastInFirstOut }

    public class Disposal : Transaction
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public CGTCalculationMethod CGTMethod { get; set; }
        public string Comment { get; set; }

        public Disposal()
            : base(Guid.NewGuid())
        {

        }

        public Disposal(Guid id)
            : base(id)
        {

        }

        protected override string GetDescription()
        {
            return "Disposed of " + Units.ToString("n0") + " shares @ " + MathUtils.FormatCurrency(AveragePrice, false, true);
        }

        protected override TransactionType GetTransactionType()
        {
            return TransactionType.Disposal;
        }

    }
}
