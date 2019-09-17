﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;

namespace PortfolioManager.RestApi.Portfolios
{
    public class CashAccountTransactionsResponse
    {
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public List<CashTransactionItem> Transactions { get; } = new List<CashTransactionItem>();

        public class CashTransactionItem
        {
            public DateTime Date { get; set; }
            public BankAccountTransactionType Type { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
