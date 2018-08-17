using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.EventStore;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class DividendAddedEvent : CorporateActionAddedEvent
    {
        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal CompanyTaxRate { get; set; }
        public decimal PercentFranked { get; set; }
        public decimal DRPPrice { get; set; }

        public DividendAddedEvent(Guid entityId, int version, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
            : base(entityId, version, actionId, actionDate, description)
        {
            PaymentDate = paymentDate;
            DividendAmount = dividendAmount;
            CompanyTaxRate = companyTaxRate;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
        }
    }
}
