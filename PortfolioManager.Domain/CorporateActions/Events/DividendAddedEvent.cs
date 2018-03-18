using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class DividendAddedEvent : IEvent
    {
        public Guid Id { get; private set; }
        public Guid ActionId { get; private set; }
        public DateTime ActionDate { get; private set; }
        public string Description { get; private set; }
        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal CompanyTaxRate { get; set; }
        public decimal PercentFranked { get; set; }
        public decimal DRPPrice { get; set; }

        public DividendAddedEvent(Guid id, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            Id = id;
            ActionId = actionId;
            ActionDate = actionDate;
            Description = description;
            PaymentDate = paymentDate;
            DividendAmount = dividendAmount;
            CompanyTaxRate = companyTaxRate;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
        }
    }
}
