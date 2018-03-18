using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class Dividend : ICorporateAction
    {
        public Guid Id { get; private set; }
        public DateTime ActionDate { get; private set; }
        public string Description { get; private set; }
        public Stock Stock { get; private set; }

        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal CompanyTaxRate { get; set; }
        public decimal PercentFranked { get; set; }
        public decimal DRPPrice { get; set; }

        public Dividend(Stock stock, Guid id, DateTime actionDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            Id = id;
            Stock = stock;
            ActionDate = actionDate;
            if (description != "")
                Description = description;
            else
                Description = "Dividend " + MathUtils.FormatCurrency(DividendAmount, false, true);

            PaymentDate = paymentDate;
            DividendAmount = dividendAmount;
            CompanyTaxRate = companyTaxRate;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
        }
    }
}
