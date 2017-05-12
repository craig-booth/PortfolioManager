using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.Service.CorporateActions
{
    interface ICorporateActionHandler
    {
        IReadOnlyCollection<Transaction> CreateTransactionList(CorporateAction corporateAction);
        bool HasBeenApplied(CorporateAction corporateAction);
    }
}
