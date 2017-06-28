using System;

namespace PortfolioManager.Data.Portfolios
{
    public class DRPCashBalance : EffectiveDatedEntity
    {
        public decimal Balance { get; set; }

        public DRPCashBalance(Guid stockId, DateTime fromDate, DateTime toDate, decimal balance)
            : base(stockId, fromDate, toDate)
        {
            Balance = balance;
        }

        public DRPCashBalance CreateNewEffectiveEntity(DateTime atDate)
        {
            return new DRPCashBalance(Id, atDate, ToDate, Balance);
        }
    }
}
