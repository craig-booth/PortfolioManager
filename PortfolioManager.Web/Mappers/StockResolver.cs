using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Web.Mapping
{
    public class StockResolver : IValueResolver<RestApi.Transactions.Transaction, Domain.Transactions.Transaction, Stock>
    {
        private IStockRepository _StockRepository;

        public StockResolver(IStockRepository stockRepository)
        {
            _StockRepository = stockRepository;
        }

        public Stock Resolve(RestApi.Transactions.Transaction source, Domain.Transactions.Transaction destination, Stock member, ResolutionContext context)
        {
            return _StockRepository.Get(source.Stock);
        }
    }
}
