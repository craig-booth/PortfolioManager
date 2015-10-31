using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    public enum AdjustmentMethod
    {
        Percentage,
        Amount
    }

    public class CostBaseAdjustment : ITransaction
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; } 
        public AdjustmentMethod Method { get; set; }
        public decimal Value { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                if (Method == AdjustmentMethod.Percentage)
                    return "Adjust cost base by " + Value.ToString("P");
                else
                    return "Adjust costbase by " + MathUtils.FormatCurrency(Value, false, true);
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
