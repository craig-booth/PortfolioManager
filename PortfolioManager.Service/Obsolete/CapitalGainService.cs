using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Obsolete
{

    public class CapitalGain
    {

        private readonly IPortfolioQuery _PortfolioQuery;

        internal CapitalGain(IPortfolioQuery portfolioQuery)
        {
            _PortfolioQuery = portfolioQuery;
        }


        public IReadOnlyCollection<CGTEvent> GetEvents(DateTime fromDate, DateTime toDate)
        {
            return _PortfolioQuery.GetCGTEvents(fromDate, toDate);
        }

    }
}
