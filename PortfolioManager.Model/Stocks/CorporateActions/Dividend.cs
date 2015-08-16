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

        public CorporateActionType Type
        {
            get
            {
                return CorporateActionType.Dividend;
            }
        }

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
                Description = "Dividend " + MathUtils.FormatCurrency(DividendAmount, false, true);
        }

        public void Change(DateTime newActionDate, DateTime newPaymentDate, decimal newDividendAmount, string newDesciption)
        {
            Change(newActionDate, newPaymentDate, newDividendAmount, PercentFranked, CompanyTaxRate, DRPPrice, newDesciption);
        }


        public void Change(DateTime newActionDate, DateTime newPaymentDate, decimal newDividendAmount, decimal newPercentFranked, decimal newCompanyTaxRate, decimal newDRPPrice, string newDesciption)
        {
            ActionDate = newActionDate;
            PaymentDate = newPaymentDate;
            DividendAmount = newDividendAmount;
            CompanyTaxRate = newCompanyTaxRate;
            PercentFranked = newPercentFranked;
            DRPPrice = newDRPPrice;
            if (newDesciption != "")
                Description = newDesciption;
            else
                Description = "Dividend " + MathUtils.FormatCurrency(DividendAmount, false, true);

            using (IStockUnitOfWork unitOfWork = _Database.CreateUnitOfWork())
            {
                unitOfWork.CorporateActionRepository.Update(this);
                unitOfWork.Save();
            }
        }

        public IReadOnlyCollection<ITransaction> CreateTransactionList(Portfolio forPortfolio)
        {
            var transactions = new List<ITransaction>();

            /* locate parcels that the dividend applies to */
            var parcels = forPortfolio.GetParcels(Stock, ActionDate);
            if (parcels.Count == 0)
                return transactions;

            var stock = _Database.StockQuery.Get(this.Stock, this.PaymentDate);

            var unitsHeld = parcels.Sum(x => x.Units);
            var amountPaid = unitsHeld * DividendAmount;
            var franked = MathUtils.ToCurrency(amountPaid * PercentFranked, stock.DividendRoundingRule);
            var unFranked = MathUtils.ToCurrency(amountPaid * (1 - PercentFranked), stock.DividendRoundingRule);
            var frankingCredits = MathUtils.ToCurrency(((amountPaid / (1 - CompanyTaxRate)) - amountPaid) * PercentFranked, stock.DividendRoundingRule);

            /* add drp shares */
            if (DRPPrice != 0.00M)
            {
                int drpUnits = (int)Math.Round(amountPaid / DRPPrice);

                transactions.Add(new OpeningBalance()
                {
                    TransactionDate = PaymentDate,
                    ASXCode = stock.ASXCode,
                    Units = drpUnits,
                    CostBase = amountPaid,
                    Comment = "DRP " + MathUtils.FormatCurrency(DRPPrice, false, true)
                }
                );
            }

            transactions.Add(new IncomeReceived()
            {
                ASXCode = stock.ASXCode,
                TransactionDate = PaymentDate,
                FrankedAmount = franked,
                UnfrankedAmount = unFranked,
                FrankingCredits = frankingCredits,
                Comment = Description
            });

            return transactions;
        }
    }
}
