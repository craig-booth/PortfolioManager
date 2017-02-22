using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{

    class PortfolioSummaryService : IPortfolioSummaryService
    {
        private readonly ShareHoldingService _ShareHoldingService;
        private readonly CashAccountService _CashAccountService;

        public PortfolioSummaryService(ShareHoldingService shareHoldingService, CashAccountService cashAccountService)
        {
            _ShareHoldingService = shareHoldingService;
            _CashAccountService = cashAccountService;
        }

        public Task<PortfolioSummaryResponce> GetSummary(DateTime date)
        {
            var response = new PortfolioSummaryResponce();

            var holdings = _ShareHoldingService.GetHoldings(date);
            var cashBalance = _CashAccountService.GetBalance(date);

            response.PortfolioValue = holdings.Sum(x => x.MarketValue) + cashBalance; 
            response.PortfolioCost = holdings.Sum(x => x.TotalCostBase) + cashBalance;

            response.CashBalance = cashBalance;

            response.Return1Year = CalculateIRR(date, 1);
            response.Return3Year = CalculateIRR(date, 3);
            response.Return5Year = CalculateIRR(date, 5);
            response.ReturnAll = CalculateIRR(date, 0);

            foreach (var holding in holdings)
                response.Holdings.Add(new Holding()
                {
                    ASXCode = holding.Stock.ASXCode,
                    CompanyName = holding.Stock.Name, 
                    Category = holding.Stock.Category,
                    Units = holding.Units,
                    Value = holding.MarketValue,
                    Cost = holding.TotalCostBase
                });

            return Task.FromResult<PortfolioSummaryResponce>(response);
        }

        private decimal? CalculateIRR(DateTime date, int years)
        {
            var portfolioStartDate = _ShareHoldingService.GetPortfolioStartDate();

            DateTime startDate;
            if (years == 0)
                startDate = portfolioStartDate;
            else
                startDate = date.AddYears(-years);

            if (startDate >= portfolioStartDate)
            {
                try
                {
                   return _ShareHoldingService.CalculateIRR(startDate, date);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            } 

        }


    }





}
