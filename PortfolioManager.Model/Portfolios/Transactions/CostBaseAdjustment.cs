﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class CostBaseAdjustment : Transaction
    {
        public DateTime RecordDate { get; set; }
        public decimal Percentage { get; set; }
        public string Comment { get; set; }

        public CostBaseAdjustment()
            : base(Guid.NewGuid())
        {

        }

        public CostBaseAdjustment(Guid id)
            : base(id)
        {

        }

        protected override string GetDescription()
        {
            return "Adjust cost base by " + Percentage.ToString("P");
        }

        protected override TransactionType GetTransactionType()
        {
            return TransactionType.CostBaseAdjustment;
        }
    }
}
