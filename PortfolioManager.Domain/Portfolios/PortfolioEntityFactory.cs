﻿using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Portfolios
{
    public class PortfolioEntityFactory : IEntityFactory<Portfolio>
    {
        private IStockResolver _StockResolver;

        public PortfolioEntityFactory(IStockResolver stockResolver)
        {
            _StockResolver = stockResolver;
        }

        public Portfolio Create(Guid id, string storedEntityType)
        {
            return new Portfolio(id, _StockResolver);
        }
    }
}
