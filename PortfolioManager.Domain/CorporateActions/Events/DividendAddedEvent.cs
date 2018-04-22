using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.CorporateActions.Events
{
    public class DividendAddedEvent : IEvent
    {
        public Guid Id { get; }
        public int Version { get; }
        public Guid ActionId { get; }
        public DateTime ActionDate { get; }
        public string Description { get;}
        public DateTime PaymentDate { get; }
        public decimal DividendAmount { get; }
        public decimal CompanyTaxRate { get; }
        public decimal PercentFranked { get; }
        public decimal DRPPrice { get; }

        public DividendAddedEvent(Guid id, int version, Guid actionId, DateTime actionDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            Id = id;
            Version = version;
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
