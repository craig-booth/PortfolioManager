using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service
{
    public class Income
    {
        public Stock Stock { get; set; }
        public decimal FrankedAmount { get; set; }
        public decimal UnfrankedAmount { get; set; }
        public decimal FrankingCredits { get; set; }
        public decimal Interest { get; set; }
        public decimal TaxDeferred { get; set; }

        public Income(Stock stock, decimal frankedAmount, decimal unfrankedAmount, decimal frankingCredits)
            :this(stock, frankedAmount, unfrankedAmount, frankingCredits, 0.00m, 0.00m)
        {
            
        }

        public Income(Stock stock, decimal frankedAmount, decimal unfrankedAmount, decimal frankingCredits, decimal interest, decimal taxDeferred)
        {
            Stock = stock;
            FrankedAmount = frankedAmount;
            UnfrankedAmount = unfrankedAmount;
            FrankingCredits = frankingCredits;
            Interest = interest;
            TaxDeferred = taxDeferred;
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
        private readonly StockService _StockService;

        internal IncomeService(IPortfolioQuery portfolioQuery, StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
        }


        public IReadOnlyCollection<Income> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var incomeTransactions = _PortfolioQuery.GetTransactions(TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();

            var transactionsByASXCode = from incomeReceived in incomeTransactions 
                              group incomeReceived by incomeReceived.ASXCode into g
                              orderby g.Key
                              select g;

            var result = new List<Income>();
            foreach (var incomeReceivedTransactions in transactionsByASXCode)
            {
                var stock = _StockService.Get(incomeReceivedTransactions.Key, toDate);
                var frankedAmount = incomeReceivedTransactions.Sum(x => x.FrankedAmount);
                var unfrankedAmount = incomeReceivedTransactions.Sum(x => x.UnfrankedAmount);
                var frankingCredits = incomeReceivedTransactions.Sum(x => x.FrankingCredits);
                var interest = incomeReceivedTransactions.Sum(x => x.Interest);
                var taxDeferred = incomeReceivedTransactions.Sum(x => x.TaxDeferred);

                result.Add(new Income(stock, frankedAmount, unfrankedAmount, frankingCredits, interest, taxDeferred));
            }

            return result;
        }
    }
}
