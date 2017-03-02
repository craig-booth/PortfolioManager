using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Data;

namespace PortfolioManager.Model.Portfolios
{
    public class ShareParcelAudit : Entity
    {
        public Guid Parcel { get; private set; }
        public DateTime Date { get; private set; }
        public Guid Transaction { get; private set; }

        public int UnitCount { get; private set; }
        public decimal CostBaseChange { get; private set; }
        public decimal AmountChange { get; private set; }

        public ShareParcelAudit(Guid parcel, DateTime date, Guid transaction, int newUnitCount, decimal costBaseChange, decimal amountChange)
            : base(Guid.NewGuid())
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
