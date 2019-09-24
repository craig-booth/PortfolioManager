using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{
    public interface IPortfolioSummaryService
    {
        PortfolioSummaryResponse GetSummary(Guid portfolioId, DateTime date);
    }

    public class PortfolioSummaryService : IPortfolioSummaryService
    {
        private readonly IPortfolioCache _PortfolioCache;

        public PortfolioSummaryService(IPortfolioCache portfolioCache)
        {
            _PortfolioCache = portfolioCache;
        }

        public PortfolioSummaryResponse GetSummary(Guid portfolioId, DateTime date)
        {
            var portfolio = _PortfolioCache.Get(portfolioId);

            var response = new PortfolioSummaryResponse();
            response.Holdings.AddRange(portfolio.Holdings.All(date).Select(x => x.Convert(date)));
            response.CashBalance = portfolio.CashAccount.Balance(date);
            response.PortfolioValue = response.Holdings.Sum(x => x.Value) + response.CashBalance;
            response.PortfolioCost = response.Holdings.Sum(x => x.Cost) + response.CashBalance;

            response.Return1Year = null;
            response.Return3Year = null;
            response.Return5Year = null;
            response.ReturnAll = null;
            if (portfolio.StartDate != DateUtils.NoStartDate)
            {
                var fromDate = date.AddYears(-1).AddDays(1);
                if (fromDate >= portfolio.StartDate)
                    response.Return1Year = portfolio.CalculateIRR(new DateRange(fromDate, date));

                fromDate = date.AddYears(-3).AddDays(1);
                if (fromDate >= portfolio.StartDate)
                    response.Return3Year = portfolio.CalculateIRR(new DateRange(fromDate, date));

                fromDate = date.AddYears(-5).AddDays(1);
                if (fromDate >= portfolio.StartDate)
                    response.Return5Year = portfolio.CalculateIRR(new DateRange(fromDate, date));

                if (date >= portfolio.StartDate)
                    response.ReturnAll = portfolio.CalculateIRR(new DateRange(portfolio.StartDate, date));
            }

            return response;
        }
    }
}
