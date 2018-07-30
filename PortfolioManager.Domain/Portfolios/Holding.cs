using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Domain.Portfolios
{
    public class Holding : EffectiveEntity
    {
        public Stock Stock { get; set; }

        public EffectiveProperties<HoldingProperties> Properties = new EffectiveProperties<HoldingProperties>();

        private Dictionary<Guid, Parcel> _Parcels = new Dictionary<Guid, Parcel>();

        public Holding(Stock stock, DateTime fromDate)
            : base(stock.Id, fromDate)
        {
            Stock = stock;
            
            Properties.Change(fromDate, new HoldingProperties(0, 0.00m, 0.00m));
        }

        public void AddParcel(DateTime date, DateTime aquisitionDate, int units, decimal unitPrice, decimal amount, decimal costBase)
        {
            var parcel = new Parcel(Guid.NewGuid(), date, aquisitionDate, new ParcelProperties(units, unitPrice, amount, costBase));

            _Parcels.Add(parcel.Id, parcel);

            var exisingProperties = Properties[date];
            var newProperties = new HoldingProperties(exisingProperties.Units + units, exisingProperties.Amount + amount, exisingProperties.CostBase + costBase);
            Properties.Change(date, newProperties);
        }
    }

    public struct HoldingProperties
    {
        public readonly int Units;
        public readonly decimal Amount;
        public readonly decimal CostBase;

        public HoldingProperties(int units, decimal amount, decimal costBase)
        {
            Units = units;
            Amount = amount;
            CostBase = costBase;
        }
    }
}
