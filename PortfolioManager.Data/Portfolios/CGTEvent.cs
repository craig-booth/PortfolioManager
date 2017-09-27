using System;

using PortfolioManager.Common;

namespace PortfolioManager.Data.Portfolios
{

    public class CGTEvent: Entity
    {
        public Guid Stock { get; set; }
        public int Units { get; set; }
        public DateTime EventDate { get; set; }
        public decimal CostBase { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal CapitalGain { get; set; }
        public CGTMethod CGTMethod { get; set; }

        public CGTEvent() 
            : this(Guid.NewGuid())
        {

        }

        public CGTEvent(Guid id)
            : base(id)
        {
 
        }

        public CGTEvent(Guid stock, DateTime eventDate, int units, decimal costBase, decimal amountReceived, decimal capitalGain, CGTMethod cgtMethod)
            : this()
        {      
            Stock = stock;
            Units = units;
            EventDate = eventDate;
            CostBase = costBase;
            AmountReceived = amountReceived;
            CapitalGain = capitalGain;
            CGTMethod = cgtMethod;
        }


    }
}
