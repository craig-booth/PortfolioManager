using System;

namespace PortfolioManager.Data.Portfolios
{
    public class ShareParcelAudit : Entity
    {
        public Guid Parcel { get; set; }
        public DateTime Date { get; set; }
        public Guid Transaction { get; set; }

        public int UnitCount { get; set; }
        public decimal CostBaseChange { get; set; }
        public decimal AmountChange { get; set; }

        public ShareParcelAudit(Guid id)
           : base(id)
        {

        }

        public ShareParcelAudit(Guid parcel, DateTime date, Guid transaction, int newUnitCount, decimal costBaseChange, decimal amountChange)
            : this(Guid.NewGuid())
        {
            Parcel = parcel;
            Date = date;
            Transaction = transaction;
            UnitCount = newUnitCount;
            CostBaseChange = costBaseChange;
            AmountChange = amountChange;
        }

    }
}
