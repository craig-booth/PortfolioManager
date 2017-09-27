using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{
    public class OpeningBalance : Transaction
    {
        public int Units { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }
        public Guid PurchaseId { get; set; }

        public OpeningBalance()
            : this(Guid.NewGuid())
        {
            Type = TransactionType.OpeningBalance;
        }

        public OpeningBalance(Guid id)
            : base(id)
        {
            Type = TransactionType.OpeningBalance;
        }

        protected override string GetDescription()
        {
            return "Opening balance of " + Units.ToString("n0") + " shares";
        }
    }
}
