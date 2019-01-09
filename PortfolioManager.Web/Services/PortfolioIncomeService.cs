using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Services
{
    public class PortfolioIncomeService
    {
        public Portfolio Portfolio { get; }

        public PortfolioIncomeService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public IncomeResponse GetIncome(DateRange dateRange)
        {
            var response = new IncomeResponse();

            var incomes = Portfolio.Transactions.InDateRange(dateRange)
                .Where(x => x is IncomeReceived)
                .Select(x => x as IncomeReceived)
                .GroupBy(x => x.Stock,
                        x => x,
                        (key, result) => new IncomeResponse.IncomeItem()
                        {
                            Stock = key.Convert(dateRange.ToDate),
                            UnfrankedAmount = result.Sum(x => x.UnfrankedAmount),
                            FrankedAmount = result.Sum(x => x.FrankedAmount),
                            FrankingCredits = result.Sum(x => x.FrankingCredits),
                            NettIncome = result.Sum(x => x.CashIncome),
                            GrossIncome = result.Sum(x => x.TotalIncome)
                        });

            response.Income.AddRange(incomes);

            return response;
        }
    }
}
