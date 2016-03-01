using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Stocks
{
    public class Dividend : ICorporateAction
    {
        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal DividendAmount { get; set; }
        public decimal CompanyTaxRate { get; set; }
        public decimal PercentFranked { get; set; }
        public decimal DRPPrice { get; set; }
        public string Description { get; set; }

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.Dividend;
            }
        }

        public Dividend(Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, string description)
            : this(Guid.NewGuid(), stock, actionDate, paymentDate, amount, percentFranked, companyTaxRate, 0.00m, description)
        {
        }

        public Dividend(Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, decimal drpPrice, string description)
            : this(Guid.NewGuid(), stock, actionDate, paymentDate, amount, percentFranked, companyTaxRate, drpPrice, description)
        {
        }

        public Dividend(Guid id, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, decimal drpPrice, string description)
        {
            Id = id;
            ActionDate = actionDate;
            Stock = stock;
            PaymentDate = paymentDate;
            DividendAmount = amount;
            CompanyTaxRate = companyTaxRate;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
            if (description != "")
                Description = description;
            else
                Description = "Dividend " + MathUtils.FormatCurrency(DividendAmount, false, true);
        }
    }
}
