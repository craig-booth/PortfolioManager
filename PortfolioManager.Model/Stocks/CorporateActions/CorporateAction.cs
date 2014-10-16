using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public interface ICorporateAction : IEntity
    {
        Guid Stock { get; }
        DateTime ActionDate { get; }
        string Description { get; }
    }
}
