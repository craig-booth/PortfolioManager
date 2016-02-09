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

        public Income(string asxCode, decimal frankedAmount, decimal unfrankedAmount, decimal frankingCredits)
            :this(asxCode, frankedAmount, unfrankedAmount, frankingCredits, 0.00m, 0.00m)
        {
            
        }

        public Income(string asxCode, decimal frankedAmount, decimal unfrankedAmount, decimal frankingCredits, decimal interest, decimal taxDeferred)
        {
            ASXCode = asxCode;
            FrankedAmount = frankedAmount;
            UnfrankedAmount = unfrankedAmount;
            FrankingCredits = frankingCredits;
            Interest = interest;
            TaxDeferred = taxDeferred;
        }

        public Income(IncomeReceived incomeReceived)
            : this(incomeReceived.ASXCode, incomeReceived.FrankedAmount, incomeReceived.UnfrankedAmount, incomeReceived.FrankingCredits, incomeReceived.Interest, incomeReceived.TaxDeferred)
        {

        }

        public Income(IEnumerable<IncomeReceived> incomeReceivedTransactions)
            : this(incomeReceivedTransactions.First().ASXCode, 0.00m, 0.00m, 0.00m, 0.00m, 0.00m)
        {
            foreach (var incomeReceived in incomeReceivedTransactions)
            {
                FrankedAmount += incomeReceived.FrankedAmount;
                UnfrankedAmount += incomeReceived.UnfrankedAmount;
                FrankingCredits += incomeReceived.FrankingCredits;
                Interest += incomeReceived.Interest;
                TaxDeferred += incomeReceived.TaxDeferred;
            }                   
        }

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
            var incomeTransactions = _PortfolioQuery.GetTransactions(Guid.Empty, TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();

            var transactionsByASXCode = from incomeReceived in incomeTransactions 
                              group incomeReceived by incomeReceived.ASXCode into g
                              orderby g.Key
                              select g;

            var result = new List<Income>();
            foreach (var incomeReceivedTransactions in transactionsByASXCode)
            {
                result.Add(new Income(incomeReceivedTransactions));
            }

            return result;
        }
    }
}
