using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{

    public interface IPortfolioIncomeService
    {
        IncomeResponse GetIncome(Guid portfolioId, DateRange dateRange);
    }

    public class PortfolioIncomeService : IPortfolioIncomeService
    {
        private readonly IPortfolioCache _PortfolioCache;

        public PortfolioIncomeService(IPortfolioCache portfolioCache)
        {
            _PortfolioCache = portfolioCache;
        }

        public IncomeResponse GetIncome(Guid portfolioId, DateRange dateRange)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var response = new IncomeResponse();

            var incomes = portfolio.Transactions.InDateRange(dateRange).OfType<IncomeReceived>()
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
