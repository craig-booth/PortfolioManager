using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Obsolete
{
    class Income
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

    class IncomeService
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

            var result = new List<Income>();
            foreach (var transaction in incomeTransactions)
            {
                var stock = _StockService.Get(transaction.ASXCode, transaction.RecordDate);
                var income = result.Find(x => x.Stock.Id == stock.Id);
                if (income == null)
                {
                    income = new Income(stock, 0.00m, 0.00m, 0.00m, 0.00m, 0.00m);
                    result.Add(income);
                }

                income.FrankedAmount += transaction.FrankedAmount;
                income.UnfrankedAmount += transaction.UnfrankedAmount;
                income.FrankingCredits += transaction.FrankingCredits;
                income.Interest += transaction.Interest;
                income.TaxDeferred += transaction.TaxDeferred;
            }

            return result.OrderBy(x => x.Stock.ASXCode).ToList();
        }

        public Income GetIncome(Stock stock, DateTime fromDate, DateTime toDate)
        {
            var incomeTransactions = _PortfolioQuery.GetTransactions(stock.ASXCode, TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();
 
            var frankedAmount = incomeTransactions.Sum(x => x.FrankedAmount);
            var unfrankedAmount = incomeTransactions.Sum(x => x.UnfrankedAmount);
            var frankingCredits = incomeTransactions.Sum(x => x.FrankingCredits);
            var interest = incomeTransactions.Sum(x => x.Interest);
            var taxDeferred = incomeTransactions.Sum(x => x.TaxDeferred);

            return new Income(stock, frankedAmount, unfrankedAmount, frankingCredits, interest, taxDeferred);
        }

        public decimal GetDRPCashBalance(Stock stock, DateTime date)
        {
            return _PortfolioQuery.GetDRPCashBalance(stock.Id, date);
        }

    }
}
