using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class Transformation : CorporateAction
    {
        public Transformation(Guid id, Stock stock, DateTime actionDate, string description)
            : base(id, stock, CorporateActionType.Transformation, actionDate, description)
        {
        }
    }
}
