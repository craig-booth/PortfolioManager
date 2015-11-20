using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class CGTService
    {

        private readonly IPortfolioQuery _PortfolioQuery;

        internal CGTService(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }


        public IReadOnlyCollection<CGTEvent> GetEvents(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioQuery.GetCGTEvents(Guid.Empty, fromDate, toDate);
        }

    }
}
