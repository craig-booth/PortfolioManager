using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Transactions;

namespace PortfolioManager.Domain.Portfolios
{
    public class ParcelAudit
    {
        public DateTime Date { get; }

        public int UnitCountChange { get; }
        public decimal CostBaseChange { get; }
        public decimal AmountChange { get; }

        public Transaction Transaction { get; }

        public ParcelAudit(DateTime date, int unitCountChange, decimal costBaseChange, decimal amountChange, Transaction transaction)
        {
            Date = date;
            UnitCountChange = unitCountChange;
            CostBaseChange = costBaseChange;
            AmountChange = amountChange;
            Transaction = transaction;
        }

    }
}
