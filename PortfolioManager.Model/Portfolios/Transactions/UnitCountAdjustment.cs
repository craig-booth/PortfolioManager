using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Model.Portfolios
{
    public class UnitCountAdjustment : ITransaction 
    {
        public Guid Id { get; private set; }
        public DateTime TransactionDate { get; set; }
        public string ASXCode { get; set; }
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
        public string Comment { get; set; }

        public string Description
        {
            get
            {
                return "Adjust unit count using ratio " + OriginalUnits.ToString("n0") + ":" + NewUnits.ToString("n0");
            }
        }
        
        public UnitCountAdjustment()
            : this (Guid.NewGuid())
        {

        }

        public UnitCountAdjustment(Guid id)
        {
            Id = id;
        }

        public TransactionType Type
        {
            get
            {
                return TransactionType.UnitCountAdjustment;
            }
        }


    }
}
