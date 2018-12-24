using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
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

        public IEnumerable<Parcel> Parcels(DateTime date)
        {
            return _Parcels.Values.Where(x => x.IsEffectiveAt(date));
        }

        public void AddParcel(DateTime date, DateTime aquisitionDate, int units, decimal amount, decimal costBase)
        {
            var parcel = new Parcel(Guid.NewGuid(), date, aquisitionDate, new ParcelProperties(units, amount, costBase));

            _Parcels.Add(parcel.Id, parcel);

            var exisingProperties = Properties[date];
            var newProperties = new HoldingProperties(exisingProperties.Units + units, exisingProperties.Amount + amount, exisingProperties.CostBase + costBase);
            Properties.Change(date, newProperties);
        }

        public void DisposeOfParcel(Guid id, DateTime date, int units, decimal amount)
        {
            var parcel = _Parcels[id];
            var parcelProperties = parcel.Properties[date];

            if (units > parcelProperties.Units)
                throw new Exception("Not enough shares in parcel");

            var costBase = 0.00m;
            if (units == parcelProperties.Units)
            {
                costBase = parcelProperties.CostBase;
                parcel.End(date);
            }
            else
            {
                costBase = (parcelProperties.CostBase * ((decimal)units / parcelProperties.Units)).ToCurrency(RoundingRule.Round);

                var newParcelProperties = new ParcelProperties(parcelProperties.Units - units, parcelProperties.Amount - amount, parcelProperties.CostBase - costBase);
                parcel.Properties.Change(date, newParcelProperties);
            }

            var existingProperties = Properties[date];
            if (units == existingProperties.Units)
            {
                End(date);
            }
            else
            {                
                var newProperties = new HoldingProperties(existingProperties.Units - units, existingProperties.Amount - amount, existingProperties.CostBase - costBase);
                Properties.Change(date, newProperties);
            }
        }

        public decimal Value(DateTime date)
        {
            if (EffectivePeriod.Contains(date))
                return Properties[date].Units * Stock.GetPrice(date);
            else
                return 0.00m;
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
