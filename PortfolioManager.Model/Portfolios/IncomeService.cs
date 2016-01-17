using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class Income
    {
        public string ASXCode { get; set; }
        public decimal FrankedAmount { get; set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get; set; }
        public decimal Interest { get; set; }
        public decimal TaxDeferred { get; set; }

        public decimal CashIncome
        {
            get { return FrankedAmount + UnfrankedAmount + Interest; }
        }

        public decimal NonCashIncome
        {
            get { return FrankingCredits + TaxDeferred; }
        }

        public decimal TotalIncome
        {
            get { return CashIncome + NonCashIncome; }
        }
    }

    public class IncomeService
    {

        private readonly IPortfolioQuery _PortfolioQuery;

        internal IncomeService(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }


        public IReadOnlyCollection<Income> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var incomeTransactions = _PortfolioQuery.GetTransactions(Guid.Empty, TransactionType.Income, fromDate, toDate);

            var incomeQuery = from incomeReceived in incomeTransactions
                              group incomeReceived by incomeReceived.ASXCode into g
                              orderby g.Key
                              select new Income()
                              {
                                    ASXCode = g.Key,
                                    FrankedAmount = g.Sum(x => (x as IncomeReceived).FrankedAmount),
                                    UnfrankedAmount = g.Sum(x => (x as IncomeReceived).UnfrankedAmount),
                                    FrankingCredits = g.Sum(x => (x as IncomeReceived).FrankingCredits),
                                    Interest = g.Sum(x => (x as IncomeReceived).Interest),
                                    TaxDeferred = g.Sum(x => (x as IncomeReceived).TaxDeferred)
                              };

            return incomeQuery.ToList().AsReadOnly();

        }
    }
}
