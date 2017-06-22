using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Data;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Local
{
    class HoldingService : IHoldingService
    {
        private readonly PortfolioUtils _PortfolioUtils;

        public HoldingService(IPortfolioQuery portfolioQuery, IStockQuery stockQuery)
        {
            _PortfolioUtils = new PortfolioUtils(portfolioQuery, stockQuery);
        }

        public Task<HoldingResponce> GetHolding(Guid stock, DateTime date)
        {
            var responce = new HoldingResponce();

            responce.Holding = _PortfolioUtils.GetHolding(stock, date);
         
            responce.SetStatusToSuccessfull();

            return Task.FromResult<HoldingResponce>(responce);
        }

        public Task<HoldingsResponce> GetHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            responce.Holdings.AddRange(_PortfolioUtils.GetHoldings(date));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<HoldingsResponce>(responce);
        }


        public Task<HoldingsResponce> GetTradeableHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            responce.Holdings.AddRange(_PortfolioUtils.GetTradeableHoldings(date));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<HoldingsResponce>(responce);
        }
    }
}
