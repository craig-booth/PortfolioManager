using System;
using System.Linq;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Local
{

    public class PortfolioSummaryService : IPortfolioSummaryService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;       

        public PortfolioSummaryService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;
        }

        public Task<PortfolioPropertiesResponce> GetProperties()
        {
            var responce = new PortfolioPropertiesResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    responce.StartDate = PortfolioUtils.GetPortfolioStartDate(portfolioUnitOfWork.PortfolioQuery);
                    responce.EndDate = DateUtils.NoEndDate;   // This is wrong !!!!

                    var stocksOwned = portfolioUnitOfWork.PortfolioQuery.GetStocksOwned(responce.StartDate, responce.EndDate);
                    foreach (var id in stocksOwned)
                    {
                        var stock = stockUnitOfWork.StockQuery.Get(id, responce.EndDate);
                        if (stock.ParentId == Guid.Empty)
                            responce.StocksHeld.Add(new StockItem(stock));
                    }

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<PortfolioPropertiesResponce>(responce);
        }

        public Task<PortfolioSummaryResponce> GetSummary(DateTime date)
        {
            var responce = new PortfolioSummaryResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    var holdings = PortfolioUtils.GetHoldings(date, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);
                    var cashBalance = portfolioUnitOfWork.PortfolioQuery.GetCashBalance(date);

                    responce.PortfolioValue = holdings.Sum(x => x.Value) + cashBalance;
                    responce.PortfolioCost = holdings.Sum(x => x.Cost) + cashBalance;

                    responce.CashBalance = cashBalance;

                    responce.Return1Year = CalculateIRR(date, 1, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);
                    responce.Return3Year = CalculateIRR(date, 3, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);
                    responce.Return5Year = CalculateIRR(date, 5, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);
                    responce.ReturnAll = CalculateIRR(date, 0, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

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
                }
            }

            return Task.FromResult<PortfolioSummaryResponce>(responce);
        }

        private decimal? CalculateIRR(DateTime date, int years, IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            var portfolioStartDate = PortfolioUtils.GetPortfolioStartDate(portfolioQuery);

            DateTime startDate;
            if (years == 0)
                startDate = portfolioStartDate;
            else
                startDate = date.AddYears(-years);

            if (startDate >= portfolioStartDate)
            {
                try
                {
                   return PortfolioUtils.CalculatePortfolioIRR(startDate, date, portfolioQuery, stockQuery);
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
