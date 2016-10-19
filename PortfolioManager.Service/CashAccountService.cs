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
            var transactons = GetTransactions(DateUtils.NoStartDate, atDate);

            return transactons.Sum(x => x.Amount);
        }
    }
}
