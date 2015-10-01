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
            return false;
        }

        public void Write(MessageWriter writer, CGTEvent cgtEvent)
        {
            writer.Write("<CGTEvent:- >");                
        }
    }

}
