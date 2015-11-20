using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class IncomeService
    {

        private readonly IPortfolioQuery _PortfolioQuery;

        internal IncomeService(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }


        public IReadOnlyCollection<IncomeReceived> GetIncome(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioQuery.GetIncome(Guid.Empty, fromDate, toDate);
        }
    }
}
