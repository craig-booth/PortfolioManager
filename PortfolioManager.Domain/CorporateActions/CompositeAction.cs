using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class CompositeAction : CorporateAction
    {
        public CompositeAction(Guid id, Stock stock, DateTime actionDate, string description)
            : base(id, stock, CorporateActionType.Composite, actionDate, description)
        {

        }
    }
}
