using System;
using System.Collections.Generic;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Stocks
{
    public class CompositeAction : CorporateAction
    {
        public List<CorporateAction> Children { get; private set; }

        public CompositeAction(Guid stock, DateTime actionDate, string description)
            : this(Guid.NewGuid(), stock, actionDate, description)
        {
        }

        public CompositeAction(Guid id, Guid stock, DateTime actionDate, string description)
            : base(id, CorporateActionType.Composite, stock, actionDate)
        {
            Description = description;

            Children = new List<CorporateAction>();
        }
        
    }
}
