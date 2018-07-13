using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.CorporateActions
{
    public class CapitalReturn : CorporateAction
    {
        public DateTime PaymentDate { get; private set; }
        public decimal Amount { get; private set; }

        public CapitalReturn(Guid id, Stock stock, DateTime actionDate, string description, DateTime paymentDate, decimal amount)
            : base(id, stock, CorporateActionType.CapitalReturn, actionDate, description)
        {
            Amount = amount;
        }
    }
}
