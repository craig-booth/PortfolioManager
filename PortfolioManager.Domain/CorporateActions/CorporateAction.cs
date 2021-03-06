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
    public abstract class CorporateAction : ITransaction
    {
        public Guid Id { get; private set; }
        public Stock Stock { get; private set; }
        public DateTime Date { get; private set; }
        public CorporateActionType Type { get; private set; }
        public string Description { get; private set; }

        internal CorporateAction(Guid id, Stock stock, CorporateActionType type, DateTime actionDate, string description)
        {
            Id = id;
            Stock = stock;
            Type = type;
            Date = actionDate;
            Description = description;
        }

        public abstract IEnumerable<Transaction> GetTransactionList(Holding holding);
        public abstract bool HasBeenApplied(ITransactionCollection transactions);
    }
}
