using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
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
