using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public interface ICorporateAction
    {
        Guid Id { get; }
        DateTime ActionDate { get; }
        string Description { get; }
        Stock Stock { get; }
    }
}
