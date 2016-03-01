using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Stocks
{
    public class CompositeAction : ICorporateAction
    {
        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public List<ICorporateAction> Children { get; private set; }
        public string Description { get; set; }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.Composite;
            }
        }

        public CompositeAction(Guid stock, DateTime actionDate, string description)
            : this(Guid.NewGuid(), stock, actionDate, description)
        {
        }

        public CompositeAction(Guid id, Guid stock, DateTime actionDate, string description)
        {
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            Description = description;

            Children = new List<ICorporateAction>();
        }
        
    }
}
