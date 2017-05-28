using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Local
{

    class PortfolioSummaryService : IPortfolioSummaryService
    {
        private readonly IPortfolioQuery _PortfolioQuery;  
        private readonly PortfolioUtils _PortfolioUtils;
        private readonly Obsolete.StockService _StockService;

        public PortfolioSummaryService(IPortfolioQuery portfolioQuery, IStockQuery stockQuery, IStockDatabase stockDatabase, Obsolete.StockService stockService)
        {
            _PortfolioQuery = portfolioQuery;
            _StockService = stockService;
            _PortfolioUtils = new PortfolioUtils(portfolioQuery, stockQuery, stockDatabase, stockService);
        }

        public Task<PortfolioPropertiesResponce> GetProperties()
        {
            var responce = new PortfolioPropertiesResponce();

            responce.StartDate = _PortfolioUtils.GetPortfolioStartDate();
            responce.EndDate = DateUtils.NoEndDate;   // This is wrong !!!!
            
            var stocksOwned =_PortfolioQuery.GetStocksOwned(responce.StartDate, responce.EndDate);
            foreach (var id in stocksOwned)
            {
                var stock = _StockService.Get(id, responce.EndDate);
                if (stock.ParentId == Guid.Empty)
                    responce.StocksHeld.Add(new StockItem(stock));
            }

            responce.SetStatusToSuccessfull();

            return Task.FromResult<PortfolioPropertiesResponce>(responce);
        }

        public Task<PortfolioSummaryResponce> GetSummary(DateTime date)
        {
            var responce = new PortfolioSummaryResponce();

            var holdings = _PortfolioUtils.GetHoldings(date);
            var cashBalance = _PortfolioQuery.GetCashBalance(date);

            responce.PortfolioValue = holdings.Sum(x => x.Value) + cashBalance; 
            responce.PortfolioCost = holdings.Sum(x => x.Cost) + cashBalance;

            responce.CashBalance = cashBalance;

            responce.Return1Year = CalculateIRR(date, 1);
            responce.Return3Year = CalculateIRR(date, 3);
            responce.Return5Year = CalculateIRR(date, 5);
            responce.ReturnAll = CalculateIRR(date, 0);

            foreach (var holding in holdings)
                responce.Holdings.Add(new HoldingItem()
                {
                    Stock = holding.Stock,
                    Category = holding.Category,
                    Units = holding.Units,
                    Value = holding.Value,
                    Cost = holding.Cost
                });

            responce.SetStatusToSuccessfull();

            return Task.FromResult<PortfolioSummaryResponce>(responce);
        }

        private decimal? CalculateIRR(DateTime date, int years)
        {
            var portfolioStartDate = _PortfolioUtils.GetPortfolioStartDate();

            DateTime startDate;
            if (years == 0)
                startDate = portfolioStartDate;
            else
                startDate = date.AddYears(-years);

            if (startDate >= portfolioStartDate)
            {
                try
                {
                   return _PortfolioUtils.CalculatePortfolioIRR(startDate, date);
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
