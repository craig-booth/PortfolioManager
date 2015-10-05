using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;

using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Model.Test.Portfolios
{
    public class CGTEventComparer : IEntityComparer<CGTEvent>
    {
        public bool Equals(CGTEvent cgtEvent1, CGTEvent cgtEvent2)
        {
            return ((cgtEvent1.Stock == cgtEvent2.Stock) &&
                    (cgtEvent1.Units == cgtEvent2.Units) &&
                    (cgtEvent1.EventDate == cgtEvent2.EventDate) &&
                    (cgtEvent1.CostBase == cgtEvent2.CostBase) &&
                    (cgtEvent1.AmountReceived == cgtEvent2.AmountReceived) &&
                    (cgtEvent1.CapitalGain == cgtEvent2.CapitalGain));
        }

    public void Write(MessageWriter writer, CGTEvent cgtEvent)
        {
            writer.Write("<CGTEvent:- Stock: {0}, Units: {1}, EventDate: {2:d}, CostBase: {3}, AmountReceived: {4}, CapitalGain: {5}>",
                new object[] {cgtEvent.Stock, cgtEvent.Units, cgtEvent.EventDate, cgtEvent.CostBase, cgtEvent.AmountReceived, cgtEvent.CapitalGain});                
        }

    }

}
