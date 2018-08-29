using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public class CompositeAction : CorporateAction
    {
        public override string Type
        {
            get { return "composite"; }
        }
    }
}
