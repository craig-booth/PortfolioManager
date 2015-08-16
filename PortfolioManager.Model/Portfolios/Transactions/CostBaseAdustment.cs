using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Model.Portfolios
{
    public class CostBaseAdjustment : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public decimal Percentage { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Adjust cost base by " + Percentage.ToString("P");
            }
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.CostBaseAdjustment;
            }
        }

        public CostBaseAdjustment()
            : this (Guid.NewGuid())
        {

        }

        public CostBaseAdjustment(Guid id)
        {
            Id = id;
        }
    }
}
