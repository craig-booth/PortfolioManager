using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Transactions;

namespace PortfolioManager.Domain.Portfolios
{
    public class Holding : EffectiveEntity
    {
        public Stock Stock { get; set; }

        private EffectiveProperties<HoldingProperties> _Properties = new EffectiveProperties<HoldingProperties>();
        public IEffectiveProperties<HoldingProperties> Properties => _Properties;

        public HoldingSettings Settings = new HoldingSettings(false);

        private Dictionary<Guid, Parcel> _Parcels = new Dictionary<Guid, Parcel>();

        private CashAccount _DrpAccount = new CashAccount();
        public ICashAccount DrpAccount => _DrpAccount;

        public Holding(Stock stock, DateTime fromDate)
            : base(stock.Id)
        {
            Stock = stock;
            Start(fromDate);

            _Properties.Change(fromDate, new HoldingProperties(0, 0.00m, 0.00m));
        }

        public IEnumerable<Parcel> Parcels(DateTime date)
        {
            return _Parcels.Values.Where(x => x.IsEffectiveAt(date));
        }

        public void AddParcel(DateTime date, DateTime aquisitionDate, int units, decimal amount, decimal costBase, Transaction transaction)
        {
            var parcel = new Parcel(Guid.NewGuid(), date, aquisitionDate, new ParcelProperties(units, amount, costBase), transaction);

            _Parcels.Add(parcel.Id, parcel);

            var exisingProperties = Properties[date];
            var newProperties = new HoldingProperties(exisingProperties.Units + units, exisingProperties.Amount + amount, exisingProperties.CostBase + costBase);
            _Properties.Change(date, newProperties);
        }

        public void DisposeOfParcel(Parcel parcel, DateTime date, int units, decimal amount, Transaction transaction)
        {        
            var parcelProperties = parcel.Properties[date];

            if (units > parcelProperties.Units)
                throw new Exception("Not enough shares in parcel");

            var costBase = 0.00m;
            if (units == parcelProperties.Units)
                costBase = parcelProperties.CostBase;
            else
                costBase = (parcelProperties.CostBase * ((decimal)units / parcelProperties.Units)).ToCurrency(RoundingRule.Round);

            parcel.Change(date, -units, -amount, -costBase, transaction);

            var existingProperties = Properties[date];
            if (units == existingProperties.Units)
            {
                End(date);
            }
            else
            {                
                var newProperties = new HoldingProperties(existingProperties.Units - units, existingProperties.Amount - amount, existingProperties.CostBase - costBase);
                _Properties.Change(date, newProperties);
            }
        }

        public decimal Value(DateTime date)
        {
            if (EffectivePeriod.Contains(date))
                return Properties[date].Units * Stock.GetPrice(date);
            else
                return 0.00m;
        }

        public void AddDrpAccountAmount(DateTime date, decimal amount)
        {
            if (amount > 0.00m)
                _DrpAccount.Deposit(date, amount, "");
            else if (amount < 0.00m)
                _DrpAccount.Withdraw(date, -amount, "");
        }

        public void ChangeDrpParticipation(bool participateInDrp)
        {
            Settings.ParticipateInDrp = participateInDrp;
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

    public class HoldingSettings
    {
        public bool ParticipateInDrp { get; internal set; }

        public HoldingSettings(bool participateInDrp)
        {
            ParticipateInDrp = participateInDrp;
        }
    }
}
