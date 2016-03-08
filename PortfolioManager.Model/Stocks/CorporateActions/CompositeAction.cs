using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Stocks
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
