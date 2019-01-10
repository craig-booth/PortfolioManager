using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class SplitConsolidation : CorporateAction
    { 
        public SplitConsolidation(Guid id, Stock stock, DateTime actionDate, string description)
            : base(id, stock, CorporateActionType.SplitConsolidation, actionDate, description)
        {
        }
    }
}
