using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Service.Utils;

namespace PortfolioManager.Service.Services
{
    public class StockService : IStockService
    {
        private readonly StockExchange _StockExchange;
        private readonly ICorporateActionQuery _CorporateActionQuery;
        private readonly IMapper _Mapper;

        public StockService(StockExchange stockExchange, ICorporateActionQuery corporateActionQuery)
        {
            _StockExchange = stockExchange;
            _CorporateActionQuery = corporateActionQuery;

            var config = new MapperConfiguration(cfg =>
                    cfg.AddProfile(new ModelToServiceMapping(_StockExchange))
            );
            _Mapper = config.CreateMapper();
        }

        public Task<GetStockResponce> GetStocks(DateTime date, bool includeStapledSecurities, bool includeChildStocks)
        {
            var responce = new GetStockResponce();

            responce.Stocks.AddRange(_StockExchange.Stocks.All(date).Select(x => x.ToStockItem(date)));

            return Task.FromResult<GetStockResponce>(responce); 
        }

        public Task<CorporateActionsResponce> GetCorporateActions(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var responce = new CorporateActionsResponce();

            var corporateActions = _CorporateActionQuery.Find(stock, fromDate, toDate);
            responce.CorporateActions.AddRange(_Mapper.Map<IEnumerable<CorporateActionItem>>(corporateActions));

            return Task.FromResult<CorporateActionsResponce>(responce);
        }
    }
}
