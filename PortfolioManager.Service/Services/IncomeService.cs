using System;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;

        public IncomeService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;
        }

        public Task<IncomeResponce> GetIncome(DateTime fromDate, DateTime toDate)
        {
            var responce = new IncomeResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                var incomeTransactions = portfolioUnitOfWork.PortfolioQuery.GetTransactions(TransactionType.Income, fromDate, toDate).Cast<IncomeReceived>();

                var incomeSummary = from income in incomeTransactions
                                    group income by income.ASXCode into incomeGroup
                                    select new IncomeItem()
                                    {
                                        Stock = _StockExchange.Stocks.Get(incomeGroup.Key, toDate).ToStockItem(toDate),
                                        UnfrankedAmount = incomeGroup.Sum(x => x.UnfrankedAmount),
                                        FrankedAmount = incomeGroup.Sum(x => x.FrankedAmount),
                                        FrankingCredits = incomeGroup.Sum(x => x.FrankingCredits),
                                        TotalAmount = incomeGroup.Sum(x => x.TotalIncome)
                                    };

                responce.Income.AddRange(incomeSummary);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<IncomeResponce>(responce); 
        }

    }
}
