using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.RestApi.CorporateActions
{
    public class Dividend : CorporateAction
    {
        public override string Type
        {
            get { return "dividend"; }
        }
        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal PercentFranked { get; set; }
        public decimal DRPPrice { get; set; }
    }
}
