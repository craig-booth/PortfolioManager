using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{
    public class Aquisition : Transaction 
    {
        public int Units { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal TransactionCosts { get; set; }
        public bool CreateCashTransaction { get; set; }

        public Aquisition()
            : this (Guid.NewGuid())
        {
            
        }

        public Aquisition(Guid id)
            : base(id)
        {
            Type = TransactionType.Aquisition;
        }

        protected override string GetDescription()
        {
            return "Aquired " + Units.ToString("n0") + " shares @ " + MathUtils.FormatCurrency(AveragePrice, false, true);
        }

    }
}
