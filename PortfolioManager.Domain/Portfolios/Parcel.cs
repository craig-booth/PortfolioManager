using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Domain.Portfolios
{
    public class Parcel : EffectiveEntity
    {
        public DateTime AquisitionDate { get; private set; }

        public EffectiveProperties<ParcelProperties> Properties { get; } = new EffectiveProperties<ParcelProperties>();

        public Parcel(Guid id, DateTime fromDate, DateTime aquisitionDate, ParcelProperties properties)
            : base(id, fromDate)
        {
            AquisitionDate = aquisitionDate;
            Properties.Change(fromDate, properties);
        }
    }

    public struct ParcelProperties
    {
        public readonly int Units;
        public readonly decimal UnitPrice;
        public readonly decimal Amount;
        public readonly decimal CostBase;

        public ParcelProperties(int units, decimal unitPrice, decimal amount, decimal costBase)
        {
            Units = units;
            UnitPrice = unitPrice;
            Amount = amount;
            CostBase = costBase;
        }
    }
}
