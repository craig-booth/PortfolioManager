using System;
using System.Collections.Generic;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Stocks
{

    public abstract class CorporateAction : Entity
    {
        public CorporateActionType Type { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public string Description { get; set; }

        public CorporateAction(Guid id, CorporateActionType type, Guid stock, DateTime actionDate)
            : base(id)
        {
            Stock = stock;
            Type = type;
            Stock = stock;
            ActionDate = actionDate;
            Description = "";          
        }
    }

    public class CorporateActionComparer : IComparer<CorporateAction>
    {
       public int Compare(CorporateAction x, CorporateAction y) 
       {
           return DateTime.Compare(x.ActionDate, y.ActionDate);
       }
    }
}
