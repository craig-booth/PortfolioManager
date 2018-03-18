using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Stocks.Commands
{
    public class AddDividendCommand : ICommand
    {
        public string ASXCode { get; }
        public DateTime RecordDate { get; }
        public string Description { get; }
        public DateTime PaymentDate { get; }
        public decimal DividendAmount { get; }  
        public decimal CompanyTaxRate { get; }
        public decimal PercentFranked { get; }
        public decimal DRPPrice { get; }

        public AddDividendCommand(string asxCode, DateTime recordDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            ASXCode = asxCode;
            RecordDate = recordDate;
            Description = description;
            PaymentDate = paymentDate;
            DividendAmount = dividendAmount;
            CompanyTaxRate = companyTaxRate;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
        }
    }
}
