﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{

    public class ReturnOfCapital : Transaction
    {
        public DateTime RecordDate { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }

        public ReturnOfCapital()
            : this (Guid.Empty)
        {

        }

        public ReturnOfCapital(Guid id)
            : base(id)
        {
            Type = TransactionType.ReturnOfCapital;
        }

        protected override string GetDescription()
        {
            return "Return of Capital of " + MathUtils.FormatCurrency(Amount, false, true);
        }
    }
}
