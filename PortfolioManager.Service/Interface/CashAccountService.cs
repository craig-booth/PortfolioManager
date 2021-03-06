﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface ICashAccountService 
    {
        Task<CashAccountTransactionsResponce> GetTranasctions(DateTime fromDate, DateTime toDate);
    }


    public class CashAccountTransactionsResponce : ServiceResponce
    {
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public List<CashAccountTransactionItem> Transactions;

        public CashAccountTransactionsResponce()
        {
            Transactions = new List<CashAccountTransactionItem>();
        }
    }

    public class CashAccountTransactionItem
    {
        public DateTime Date { get; set; }
        public BankAccountTransactionType Type { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
