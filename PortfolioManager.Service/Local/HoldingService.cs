using System;
using System.Threading.Tasks;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Local
{
    class HoldingService : IHoldingService
    {
        private readonly IPortfolioDatabase _PortfolioDatabase;
        private readonly IStockDatabase _StockDatabase;

        public HoldingService(IPortfolioDatabase portfolioDatabase, IStockDatabase stockDatabase)
        {
            _PortfolioDatabase = portfolioDatabase;
            _StockDatabase = stockDatabase;
        }

        public Task<HoldingResponce> GetHolding(Guid stock, DateTime date)
        {
            var responce = new HoldingResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    responce.Holding = PortfolioUtils.GetHolding(stock, date, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery);

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<HoldingResponce>(responce);
        }

        public Task<HoldingsResponce> GetHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    responce.Holdings.AddRange(PortfolioUtils.GetHoldings(date, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery));

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<HoldingsResponce>(responce);
        }


        public Task<HoldingsResponce> GetTradeableHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            using (var portfolioUnitOfWork = _PortfolioDatabase.CreateReadOnlyUnitOfWork())
            {
                using (var stockUnitOfWork = _StockDatabase.CreateReadOnlyUnitOfWork())
                {
                    responce.Holdings.AddRange(PortfolioUtils.GetTradeableHoldings(date, portfolioUnitOfWork.PortfolioQuery, stockUnitOfWork.StockQuery));

                    responce.SetStatusToSuccessfull();
                }
            }

            return Task.FromResult<HoldingsResponce>(responce);
        }
    }
}
