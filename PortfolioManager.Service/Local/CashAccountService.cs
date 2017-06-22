using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;
using PortfolioManager.Data;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Local
{
    class CashAccountService : ICashAccountService
    {
        private readonly IPortfolioQuery _PortfolioQuery;

        public CashAccountService(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }

        public Task<CashAccountTransactionsResponce> GetTranasctions(DateTime fromDate, DateTime toDate)
        {
            var responce = new CashAccountTransactionsResponce();

            // Get opening blance
            responce.OpeningBalance = _PortfolioQuery.GetCashBalance(fromDate.AddDays(-1));

            decimal balance = responce.OpeningBalance;

            // get transactions
            var transactions = _PortfolioQuery.GetCashAccountTransactions(fromDate, toDate);

            foreach (var transaction in transactions)
            {
                balance += transaction.Amount;
                var newItem = new CashAccountTransactionItem()
                {
                    Date = transaction.Date,
                    Type = transaction.Type,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    Balance = balance
                };

                if ((newItem.Type == BankAccountTransactionType.Interest) &&
                    (newItem.Description == ""))
                    newItem.Description = "Interest";


                responce.Transactions.Add(newItem);
            }

            responce.ClosingBalance = balance;
            

            return Task.FromResult<CashAccountTransactionsResponce>(responce);
        }
    }
}
