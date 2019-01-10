using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public class CapitalReturn : CorporateAction
    {
        public override string Type
        {
            get { return "capitalreturn"; }
        }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}


