using System;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Service.Interface;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Service.Local
{
    public class CashAccountService : ICashAccountService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        
        public CashAccountService(IPortfolioDatabase portfolioDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
        }

        public Task<CashAccountTransactionsResponce> GetTranasctions(DateTime fromDate, DateTime toDate)
        {
            var responce = new CashAccountTransactionsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                // Get opening blance
                responce.OpeningBalance = portfolioUnitOfWork.PortfolioQuery.GetCashBalance(fromDate.AddDays(-1));

                decimal balance = responce.OpeningBalance;

                // get transactions
                var transactions = portfolioUnitOfWork.PortfolioQuery.GetCashAccountTransactions(fromDate, toDate);

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

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<CashAccountTransactionsResponce>(responce);
        }
    }
}
