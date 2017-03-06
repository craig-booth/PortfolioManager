using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.Local
{
    class HoldingService : IHoldingService
    {
        private readonly Obsolete.ShareHoldingService _ShareHoldingService;
        private readonly Obsolete.StockService _StockService;

        public HoldingService(Obsolete.ShareHoldingService shareHoldingService, Obsolete.StockService stockService)
        {
            _ShareHoldingService = shareHoldingService;
            _StockService = stockService;
        }

        public Task<OwnedStocksResponce> GetOwnedStocks(DateTime date)
        {
            var responce = new OwnedStocksResponce();

            var stocks = _ShareHoldingService.GetOwnedStocks(date, true);
            responce.Stocks.AddRange(stocks.Select(x => new StockItem(x)));

            responce.SetStatusToSuccessfull();

            return Task.FromResult<OwnedStocksResponce>(responce);
        }

        public Task<HoldingResponce> GetHolding(Guid stock, DateTime date)
        {
            var responce = new HoldingResponce();

            var s = _StockService.Get(stock, date);
            var holding = _ShareHoldingService.GetHolding(s, date);

            responce.Holding = new HoldingItem()
            {
                Stock = new StockItem(s),
                Category = s.Category,
                Units = holding.Units,
                Value = holding.MarketValue,
                Cost = holding.TotalCost
            };
         
            responce.SetStatusToSuccessfull();

            return Task.FromResult<HoldingResponce>(responce);
        }

        public Task<HoldingsResponce> GetHoldings(DateTime date)
        {
            var responce = new HoldingsResponce();

            var holdings = _ShareHoldingService.GetHoldings(date);

            foreach (var holding in holdings)
            {
                var item = new HoldingItem()
                {
                    Stock = new StockItem(holding.Stock),
                    Category = holding.Stock.Category,
                    Units = holding.Units,
                    Value = holding.MarketValue,
                    Cost = holding.TotalCost
                };

                responce.Holdings.Add(item);
            }
            
            responce.SetStatusToSuccessfull();

            return Task.FromResult<HoldingsResponce>(responce);
        }
    }
}
