using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;
using PortfolioManager.Web.Mappers;

namespace PortfolioManager.Web.Services
{
    public class PortfolioSummaryService
    {
        public Portfolio Portfolio { get; }

        public PortfolioSummaryService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public PortfolioSummaryResponse GetSummary(DateTime date)
        {
            var response = new PortfolioSummaryResponse();

            response.Holdings.AddRange(Portfolio.Holdings.All(date).Select(x => x.Convert(date)));
            response.CashBalance = Portfolio.CashAccount.Balance(date);
            response.PortfolioValue = response.Holdings.Sum(x => x.Value) + response.CashBalance;
            response.PortfolioCost = response.Holdings.Sum(x => x.Cost) + response.CashBalance;

            var fromDate = date.AddYears(-1).AddDays(1);
            if (fromDate >= Portfolio.StartDate)
                response.Return1Year = Portfolio.CalculateIRR(new DateRange(fromDate, date));
            else
                response.Return1Year = null;

            fromDate = date.AddYears(-3).AddDays(1);
            if (fromDate >= Portfolio.StartDate)
                response.Return3Year = Portfolio.CalculateIRR(new DateRange(fromDate, date));
            else
                response.Return3Year = null;

            fromDate = date.AddYears(-5).AddDays(1);
            if (fromDate >= Portfolio.StartDate)
                response.Return5Year = Portfolio.CalculateIRR(new DateRange(fromDate, date));
            else
                response.Return5Year = null;

            if (date >= Portfolio.StartDate)
                response.ReturnAll = Portfolio.CalculateIRR(new DateRange(Portfolio.StartDate, date));
            else
                response.ReturnAll = null;

            return response;
        }
    }
}
