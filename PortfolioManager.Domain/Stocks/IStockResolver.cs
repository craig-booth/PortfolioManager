using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks
{
    public interface IStockResolver
    {
        Stock GetStock(Guid id);
    }
}
