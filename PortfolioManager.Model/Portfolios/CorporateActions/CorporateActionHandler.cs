using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    interface ICorporateActionHandler
    {
        IReadOnlyCollection<ITransaction> CreateTransactionList(ICorporateAction corporateAction);
    }
}
