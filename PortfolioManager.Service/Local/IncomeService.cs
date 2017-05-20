using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;


namespace PortfolioManager.Service.Local
{
    class IncomeService : IIncomeService
    {
        private readonly Obsolete.StockService _StockService;
        private readonly IPortfolioQuery _PortfolioQuery;

        public IncomeService(IPortfolioQuery portfolioQuery, Obsolete.StockService stockService)
        {
            _StockService = stockService;
            _PortfolioQuery = portfolioQuery;
        }

        public Task<IncomeResponce> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var responce = new IncomeResponce();

            var incomeTransactions = _PortfolioQuery.GetTransactions(TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();

            var incomeSummary = from income in incomeTransactions
                                group income by income.ASXCode into incomeGroup
                                select new IncomeItem()
                                {
                                    Stock = new StockItem(_StockService.Get(incomeGroup.Key, toDate)),
                                    UnfrankedAmount = incomeGroup.Sum(x => x.UnfrankedAmount),
                                    FrankedAmount = incomeGroup.Sum(x => x.FrankedAmount),
                                    FrankingCredits = incomeGroup.Sum(x => x.FrankingCredits),
                                    TotalAmount = incomeGroup.Sum(x => x.TotalIncome)
                                };

            responce.Income.AddRange(incomeSummary);
           
            responce.SetStatusToSuccessfull();

            return Task.FromResult<IncomeResponce>(responce);
        }

    }
}
