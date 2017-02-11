using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Service
{

    public class CashAccountBalance
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public decimal Balance { get; private set; }

        public CashAccountBalance(DateTime fromDate, DateTime toDate, decimal balance)
        {
            FromDate = fromDate;
            ToDate = toDate;
            Balance = balance;
        }
    }

    public class CashAccountService
    {

        private readonly IPortfolioDatabase _PortfolioDatabase;

        internal CashAccountService(IPortfolioDatabase portfolioDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
        }


        public IEnumerable<CashAccountTransaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioDatabase.PortfolioQuery.GetCashAccountTransactions(fromDate, toDate);
        }

        public decimal GetBalance(DateTime atDate)
        {
            // Sum up transactions prior to the request date
            var transactions = GetTransactions(DateUtils.NoStartDate, atDate);

            return transactions.Sum(x => x.Amount);
        }

        public IEnumerable<CashAccountBalance> GetBalance(DateTime fromDate, DateTime toDate)
        {
            var balances = new List<CashAccountBalance>();

            decimal balance = 0.00m;
            DateTime previousDate = DateUtils.NoStartDate; 

            var transactions = GetTransactions(DateUtils.NoStartDate, toDate);
            foreach (var transaction in transactions)
            {
                if (transaction.Date >= fromDate)
                {
                    balances.Add(new CashAccountBalance(previousDate, transaction.Date.AddDays(-1), balance));
                }

                balance += transaction.Amount;
                previousDate = transaction.Date;                
            }

            if (previousDate != DateUtils.NoStartDate)
            {
                balances.Add(new CashAccountBalance(previousDate, toDate, balance));
            }

            return balances;
        }
    }
}
