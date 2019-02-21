using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Portfolios
{
    public class PortfolioEntityFactory : IEntityFactory<Portfolio>
    {
        private IStockQuery _StockQuery;

        public PortfolioEntityFactory(IStockQuery stockQuery)
        {
            _StockQuery = stockQuery;
        }


        public Portfolio Create(string storedEntityType)
        {
            return new Portfolio(_StockQuery);
        }
    }
}
