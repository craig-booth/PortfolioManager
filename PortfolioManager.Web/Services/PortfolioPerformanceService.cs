using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.RestApi.Portfolios;


namespace PortfolioManager.Web.Services
{
    public class PortfolioPerformanceService
    {
        public Portfolio Portfolio { get; }

        public PortfolioPerformanceService(Portfolio portfolio)
        {
            Portfolio = portfolio;
        }

        public PortfolioPerformanceResponse GetPerformance(DateRange dateRange)
        {
            var response = new PortfolioPerformanceResponse()
            {
                OpeningBalance = 0.00m,
                Dividends = 0.00m,
                ChangeInMarketValue = 0.00m,
                OutstandingDRPAmount = 0.00m,
                ClosingBalance = 0.00m,

                OpeningCashBalance = 0.00m,
                Deposits = 0.00m,
                Withdrawls = 0.00m,
                Interest = 0.00m,
                Fees = 0.00m,
                ClosingCashBalance = 0.00m,
            };

            return response;
        }
    }
}
