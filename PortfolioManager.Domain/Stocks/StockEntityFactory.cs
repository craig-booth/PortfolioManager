using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public class StockEntityFactory : IEntityFactory<Stock>
    {
        public Stock Create(string storedEntityType)
        {
            if (storedEntityType == "Stock")
                return new Stock();
            else if (storedEntityType == "StapledSecurity")
                return new StapledSecurity();
            else
                throw new NotSupportedException();
        }
    }
}
