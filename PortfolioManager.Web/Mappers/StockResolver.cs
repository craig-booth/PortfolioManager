using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Web.Mappers
{
    public class MapperStockResolver : IValueResolver<RestApi.Transactions.Transaction, Domain.Transactions.Transaction, Stock>
    {
        private IStockResolver _StockResolver;

        public MapperStockResolver(IStockResolver stockResolver)
        {
            _StockResolver = stockResolver;
        }

        public Stock Resolve(RestApi.Transactions.Transaction source, Domain.Transactions.Transaction destination, Stock member, ResolutionContext context)
        {
            if (source.Stock == Guid.Empty)
                return null;

            return _StockResolver.GetStock(source.Stock);
        }
    }
}
