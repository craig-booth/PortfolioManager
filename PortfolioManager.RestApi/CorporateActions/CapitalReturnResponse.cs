using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public class CapitalReturnResponse : CorporateActionResponse
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}
