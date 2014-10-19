using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;

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

        public IReadOnlyCollection<ITransaction> CreateTransactionList(Portfolio forPortfolio)
        {
            var transactions = new List<ITransaction>();

            /* locate parcels that the dividend applies to */
            var parcels = forPortfolio.GetParcels(Stock, ActionDate);
            if (parcels.Count == 0)
                return transactions;

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = unitsHeld * DividendAmount;
            var franked = MathUtils.Truncate(amountPaid * PercentFranked);
            var unFranked = MathUtils.Truncate(amountPaid * (1 - PercentFranked));
            var frankingCredits = MathUtils.Truncate(((amountPaid / (1 - CompanyTaxRate)) - amountPaid) * PercentFranked);

            /* add drp shares */
            if (DRPPrice != 0.00M)
            {
                int drpUnits = (int)Math.Round(amountPaid / DRPPrice);

                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = PaymentDate,
                    ASXCode = _Database.StockQuery.Get(this.Stock).ASXCode,
                    Units = drpUnits,
                    CostBase = amountPaid,
                    Comment = "DRP"
                }
                );
            }

            transactions.Add(new IncomeReceived()
            {
                ASXCode = _Database.StockQuery.Get(this.Stock).ASXCode,
                TransactionDate = PaymentDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits
            });

            return transactions;
        }
    }
}
