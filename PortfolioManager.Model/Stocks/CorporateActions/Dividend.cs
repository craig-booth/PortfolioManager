using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Stocks
{
    public class Dividend : ICorporateAction
    {
        private IStockDatabase _Database;

        public Guid Id { get; private set; }
        public Guid Stock { get; private set; }
        public DateTime ActionDate { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal DividendAmount { get; private set; }
        public decimal CompanyTaxRate { get; private set; }
        public decimal PercentFranked { get; private set; }
        public decimal DRPPrice { get; private set; }
        public string Description { get; private set; }

        public Dividend(IStockDatabase stockDatabase, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, decimal drpPrice, string description)
            : this(stockDatabase, Guid.NewGuid(), stock, actionDate, paymentDate, amount, percentFranked, companyTaxRate, drpPrice, description)
        {
        }

        public Dividend(IStockDatabase stockDatabase, Guid id, Guid stock, DateTime actionDate, DateTime paymentDate, decimal amount, decimal percentFranked, decimal companyTaxRate, decimal drpPrice, string description)
        {
            _Database = stockDatabase;
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
                Description = "Dividend " + DividendAmount.ToString("c");
        }

        public void Change(DateTime newActionDate, DateTime newPaymentDate, decimal newDividendAmount, decimal newCompanyTaxRate, decimal newPercentFranked, decimal newDRPPrice)
        {
            ActionDate = newActionDate;
            PaymentDate = newPaymentDate;
            DividendAmount = newDividendAmount;
            CompanyTaxRate = newCompanyTaxRate;
            PercentFranked = newPercentFranked;
            DRPPrice = newDRPPrice;

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }

        public void Change(DateTime newActionDate, DateTime newPaymentDate, decimal newDividendAmount)
        {
            Change(newActionDate, newPaymentDate, newDividendAmount, CompanyTaxRate, PercentFranked, DRPPrice);
        }
    
    }
}
