using System;
using System.Threading.Tasks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Services
{
    public class HoldingService : IHoldingService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly StockExchange _StockExchange;

        public HoldingService(IPortfolioDatabase portfolioDatabase, StockExchange stockExchange)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockExchange = stockExchange;
        }

        public Task<HoldingResponce> GetHolding(Guid stock, DateTime date)
        {
            var responce = new HoldingResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                responce.Holding = PortfolioUtils.GetHolding(stock, date, portfolioUnitOfWork.PortfolioQuery, _StockExchange);

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<HoldingResponce>(responce); 
        }

        public Task<HoldingsResponce> GetHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                responce.Holdings.AddRange(PortfolioUtils.GetHoldings(date, portfolioUnitOfWork.PortfolioQuery, _StockExchange));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<HoldingsResponce>(responce); 
        }


        public Task<HoldingsResponce> GetTradeableHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                responce.Holdings.AddRange(PortfolioUtils.GetTradeableHoldings(date, portfolioUnitOfWork.PortfolioQuery, _StockExchange));

                responce.SetStatusToSuccessfull();
            }

            return Task.FromResult<HoldingsResponce>(responce); 
        }
    }
}
