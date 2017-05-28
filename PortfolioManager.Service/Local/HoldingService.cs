using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Model.Data;
using PortfolioManager.Service.Interface;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Local
{
    class HoldingService : IHoldingService
    {
        private readonly Obsolete.StockService _StockService;
        private readonly PortfolioUtils _PortfolioUtils;

        public HoldingService(IPortfolioQuery portfolioQuery, Obsolete.StockService stockService)
        {
            _StockService = stockService;
            _PortfolioUtils = new PortfolioUtils(portfolioQuery, _StockService);
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
