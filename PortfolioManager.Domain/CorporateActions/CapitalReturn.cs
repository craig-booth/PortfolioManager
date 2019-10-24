using System;
using System.Collections.Generic;
using System.Text;
using Booth.Common;

using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.CorporateActions
{
    public class CapitalReturn : CorporateAction
    {
        public DateTime PaymentDate { get; private set; }
        public decimal Amount { get; private set; }

        internal CapitalReturn(Guid id, Stock stock, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
            : base(id, stock, CorporateActionType.CapitalReturn, actionDate, description)
        {
            PaymentDate = paymentDate;
            Amount = amount;
        }

        public override IEnumerable<Transaction> GetTransactionList(Holding holding)
        {
            var transactions = new List<Transaction>();

            return transactions;
        }

        public override bool HasBeenApplied(ITransactionCollection transactions)
        {
            return false;
        }
    }
}
