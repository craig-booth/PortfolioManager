using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public enum CorporateActionType { Dividend, CapitalReturn, Transformation, SplitConsolidation}

    public interface ICorporateAction : IEntity
    {
        CorporateActionType Type { get; }
        Guid Stock { get; }
        DateTime ActionDate { get; }
        string Description { get; }
    }

    public class CorporateActionComparer : IComparer<ICorporateAction>
    {
       public int Compare(ICorporateAction x, ICorporateAction y) 
       {
           return DateTime.Compare(x.ActionDate, y.ActionDate);
       }
    }
}
