﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Transactions
{
    public class OpeningBalanceHandler : ITransactionHandler
    {
        private HoldingCollection _Holdings;
        private CashAccount _CashAccount;

        public OpeningBalanceHandler(HoldingCollection holdings, CashAccount cashAccount)
        {
            _Holdings = holdings;
            _CashAccount = cashAccount;
        }

        public void ApplyTransaction(Transaction transaction)
        {
            var openingBalance = transaction as OpeningBalance;

            var holding = _Holdings.Get(openingBalance.Stock.Id);
            if (holding == null)
            {
                holding = _Holdings.Add(openingBalance.Stock, openingBalance.TransactionDate);
            }

            holding.AddParcel(openingBalance.TransactionDate, openingBalance.AquisitionDate, openingBalance.Units, openingBalance.CostBase, openingBalance.CostBase);
        }
    }
}
