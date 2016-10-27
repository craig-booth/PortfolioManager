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
    public class ShareParcel: EffectiveDatedEntity, IEditableEffectiveDatedEntity<ShareParcel> 
    {
        public Guid Stock { get; set; }
        public DateTime AquisitionDate { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        public Guid PurchaseId { get; set; }

        private ShareParcel(Guid id, DateTime fromDate, DateTime toDate)
            : base(id, fromDate, toDate)
        {

        }

        public ShareParcel(DateTime aquisitionDate, Guid stock, int units, decimal unitPrice, decimal amount, decimal costBase)
            : this(aquisitionDate, DateUtils.NoEndDate, aquisitionDate, stock, units, unitPrice, amount, costBase, Guid.Empty)
        {
      
        }

        public ShareParcel(DateTime aquisitionDate, Guid stock, int units, decimal unitPrice, decimal amount, decimal costBase, Guid purchaseId)
            : this(aquisitionDate, DateUtils.NoEndDate, aquisitionDate, stock, units, unitPrice, amount, costBase, purchaseId)
        {

        }

        public ShareParcel(DateTime fromDate, DateTime toDate, DateTime aquisitionDate, Guid stock, int units, decimal unitPrice, decimal amount, decimal costBase, Guid purchaseId)
            : this(Guid.NewGuid(), fromDate, toDate)
        {
            Stock = stock;
            AquisitionDate = aquisitionDate;
            Amount = amount;
            Units = units;
            UnitPrice = unitPrice;
            CostBase = costBase;
            PurchaseId = purchaseId;
        }

        public ShareParcel CreateNewEffectiveEntity(DateTime atDate)
        {
            return new ShareParcel(Id, atDate, ToDate)
            {
                Stock = Stock,
                AquisitionDate = AquisitionDate,
                Amount = Amount,
                Units = Units,
                UnitPrice = UnitPrice,
                CostBase = CostBase,
                PurchaseId = PurchaseId
            };
        }

    }

}
