using System;
using System.Collections.Generic;
using System.Text;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class CapitalReturn : ICorporateAction
    {
        public Guid Id { get; private set; }
        public DateTime ActionDate { get; private set; }
        public string Description { get; private set; }
        public Stock Stock { get; private set; }

        public DateTime PaymentDate { get; private set; }
        public decimal Amount { get; private set; }

        public CapitalReturn(Stock stock, Guid id, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
        {
            Id = id;
            Stock = stock;
            ActionDate = actionDate;
            PaymentDate = paymentDate;
            Amount = amount;
            if (description != "")
                Description = description;
            else
                Description = "Capital Return " + Amount.ToString("$#,##0.00###");
        }
    }
}
