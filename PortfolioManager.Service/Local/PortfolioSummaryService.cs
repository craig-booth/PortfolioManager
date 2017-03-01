using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

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

        public Task<PortfolioPropertiesResponce> GetProperties()
        {
            var responce = new PortfolioPropertiesResponce();

            responce.StartDate = _ShareHoldingService.GetPortfolioStartDate();
            responce.EndDate = DateUtils.NoEndDate;   // This is wrong !!!!
            
            var stocksOwned = _ShareHoldingService.GetOwnedStocks(responce.StartDate, responce.EndDate, false);
            foreach (var stock in stocksOwned)
            {
                responce.StocksHeld.Add(new StockItem(stock));
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<PortfolioPropertiesResponce>(responce);
        }

        public Task<PortfolioSummaryResponce> GetSummary(DateTime date)
        {
            var responce = new PortfolioSummaryResponce();

            var holdings = _ShareHoldingService.GetHoldings(date);
            var cashBalance = _CashAccountService.GetBalance(date);

            responce.PortfolioValue = holdings.Sum(x => x.MarketValue) + cashBalance; 
            responce.PortfolioCost = holdings.Sum(x => x.TotalCostBase) + cashBalance;

            responce.CashBalance = cashBalance;

            responce.Return1Year = CalculateIRR(date, 1);
            responce.Return3Year = CalculateIRR(date, 3);
            responce.Return5Year = CalculateIRR(date, 5);
            responce.ReturnAll = CalculateIRR(date, 0);

            foreach (var holding in holdings)
                responce.Holdings.Add(new Holding()
                {
                    Stock = new StockItem(holding.Stock),
                    Category = holding.Stock.Category,
                    Units = holding.Units,
                    Value = holding.MarketValue,
                    Cost = holding.TotalCostBase
                });

            responce.SetStatusToSuccessfull();

            return Task.FromResult<PortfolioSummaryResponce>(responce);
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
