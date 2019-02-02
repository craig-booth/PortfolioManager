using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Web.Mappers
{
    public class StockResolver : IValueResolver<RestApi.Transactions.Transaction, Domain.Transactions.Transaction, Stock>
    {
        private IStockQuery _StockQuery;

        public StockResolver(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
        }

        public Stock Resolve(RestApi.Transactions.Transaction source, Domain.Transactions.Transaction destination, Stock member, ResolutionContext context)
        {
            if (source.Stock == Guid.Empty)
                return null;

            return _StockQuery.Get(source.Stock);
        }
    }
}
