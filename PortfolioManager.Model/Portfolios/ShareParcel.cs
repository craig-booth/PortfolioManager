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
    public enum ParcelEvent { Aquisition, Disposal, OpeningBalance, CostBaseReduction, UnitCountChange};

    public class ShareParcel: IEffectiveDatedEntity 
    {
        public Guid Id { get; private set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Guid Stock { get; set; }
        public DateTime AquisitionDate { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal CostBase { get; set; }
        public ParcelEvent Event { get; set; }
        public Guid PurchaseId { get; set; }

        private ShareParcel() 
        {

            Id = Guid.NewGuid();
            PurchaseId = Guid.Empty;
        }

        public ShareParcel(DateTime aquisitionDate, Guid stock, int units, decimal unitPrice, decimal amount, decimal costBase, ParcelEvent parcelEvent)
            : this()
        {
            FromDate = aquisitionDate;
            ToDate = DateTimeConstants.NoEndDate;
            Stock = stock;
            AquisitionDate = aquisitionDate;
            Amount = amount;
            Units = units;
            UnitPrice = unitPrice;
            CostBase = costBase;
            Event = parcelEvent;
        }

        public ShareParcel(DateTime aquisitionDate, Guid stock, int units, decimal unitPrice, decimal amount, decimal costBase, Guid purchaseId, ParcelEvent parcelEvent)
            : this()
        {
            FromDate = aquisitionDate;
            ToDate = DateTimeConstants.NoEndDate;
            Stock = stock;
            AquisitionDate = aquisitionDate;
            Amount = amount;
            Units = units;
            UnitPrice = unitPrice;
            CostBase = costBase;
            PurchaseId = purchaseId;
            Event = parcelEvent;
        }

        public ShareParcel Clone()
        {
            return new ShareParcel()
            {
                Id = Id,
                FromDate = FromDate,
                ToDate = ToDate,
                Stock = Stock,
                AquisitionDate = AquisitionDate,
                Amount = Amount,
                Units = Units,
                UnitPrice = UnitPrice,
                CostBase = CostBase,
                Event = Event,
                PurchaseId = PurchaseId 
            };
        }

    }

}
