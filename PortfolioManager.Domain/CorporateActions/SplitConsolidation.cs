﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.CorporateActions
{
    public class SplitConsolidation : CorporateAction
    {
        internal SplitConsolidation(Guid id, Stock stock, DateTime actionDate, string description)
            : base(id, stock, CorporateActionType.SplitConsolidation, actionDate, description)
        {
        }

        public override IEnumerable<Transaction> GetTransactionList(Holding holding)
        {
            var transactions = new List<Transaction>();

            return transactions;
        }

        public override bool HasBeenApplied(ITransactionCollection transactions)
        {
            return false;
        }
    }
}
