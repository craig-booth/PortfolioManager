using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.CorporateActions
{
    public class Dividend : CorporateAction
    {
        public DateTime PaymentDate { get; private set; }
        public decimal DividendAmount { get; private set; }
        public decimal CompanyTaxRate { get; private set; }
        public decimal PercentFranked { get; private set; }
        public decimal DRPPrice { get; private set; }

        public Dividend(Guid id, Stock stock, DateTime actionDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
            : base(id, stock, CorporateActionType.Dividend, actionDate, description)
        {
            PaymentDate = paymentDate;
            DividendAmount = dividendAmount;
            CompanyTaxRate = companyTaxRate;
            PercentFranked = percentFranked;
            DRPPrice = drpPrice;
        }

        public override IEnumerable<Transaction> GetTransactionList(Holding holding)
        {
            var transactions = new List<Transaction>();

            return transactions;
        }

        public override bool HasBeenApplied(ITransactionList<Transaction> transactions)
        {
            return transactions.InDateRange(new DateRange(PaymentDate, PaymentDate)).Any(x => x is IncomeReceived);
        }
    }
}
