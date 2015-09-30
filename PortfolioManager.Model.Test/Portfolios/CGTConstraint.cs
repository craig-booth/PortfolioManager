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

    public class CGTEventComparer : IEqualityComparer<CGTEvent>
    {
        public bool Equals(CGTEvent cgt1, CGTEvent cgt2)
        {
            /*   return income1.FromDate == income2.FromDate &&
                      income1.ToDate == income2.ToDate &&
                      income1.Stock == income2.Stock &&
                      income1.AquisitionDate == income2.AquisitionDate &&
                      income1.Units == income2.Units &&
                      income1.UnitPrice == income2.UnitPrice &&
                      income1.CostBase == income2.CostBase &&
                      income1.Event == income2.Event; */
            return false;
        }

        public int GetHashCode(CGTEvent cgt)
        {
            return cgt.Id.GetHashCode();
        }
    }

    public class CGTEventWriter : IEntityWriter<CGTEvent>
    {
        public void Write(MessageWriter writer, CGTEvent cgt)
        {
            writer.Write("<CGTEvent>");
        }
    }
}
