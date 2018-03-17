using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{

    public class Stock : EffectiveEntity
    {
        public int Version { get; protected set; } = 0;
        protected IEventStore _EventStore;

        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();
        
        public bool Trust { get; private set; }
        public EffectiveProperties<StockProperties> Properties { get; } = new EffectiveProperties<StockProperties>();
        public EffectiveProperties<DividendReinvestmentPlan> DividendReinvestmentPlan { get; } = new EffectiveProperties<DividendReinvestmentPlan>();

        public Stock(Guid id, DateTime listingDate, IEventStore eventStore)
            : base(id, listingDate)
        {
            _EventStore = eventStore;
        }

        public override string ToString()
        {
            var properties = Properties.ClosestTo(DateTime.Today);
            return String.Format("{0} - {1}", properties.ASXCode, properties.Name);
        }

        public void List(string asxCode, string name, bool trust, AssetCategory category)
        {
            var @event = new StockListedEvent(Id, asxCode, name, EffectivePeriod.FromDate, category, trust);
            Apply(@event);

            _EventStore.StoreEvent(StockRepository.StreamId, @event, Version);
        }

        public void Apply(StockListedEvent @event)
        {
            Version++;
            Trust = @event.Trust;

            var properties = new StockProperties(@event.ASXCode, @event.Name, @event.Category);
            Properties.Change(@event.ListingDate, properties);

            var drp = new DividendReinvestmentPlan(false, RoundingRule.Round, DRPMethod.Round);
            DividendReinvestmentPlan.Change(@event.ListingDate, drp);
        }

        public void DeList(DateTime date)
        {
            var @event = new StockDelistedEvent(Id, date);
            Apply(@event);

            _EventStore.StoreEvent(StockRepository.StreamId, @event, Version);
        }

        public void Apply(StockDelistedEvent @event)
        {
            Version++;
            End(@event.DelistedDate);
        }

        public void UpdateClosingPrice(DateTime date, decimal closingPrice)
        {
            // Check that the date is within the effective period
            if (! EffectivePeriod.Contains(date))
                throw new Exception(String.Format("Stock not active on {0}", date));

            var @event = new ClosingPriceAddedEvent(Id, date, closingPrice);
            Apply(@event);

            _EventStore.StoreEvent(StockRepository.StreamId, @event, Version);
        }

        public void Apply(ClosingPriceAddedEvent @event)
        {
            Version++;
            UpdatePrice(@event.Date, @event.ClosingPrice);
        }

        public void UpdateCurrentPrice(decimal currentPrice)
        {
            // Check that the date is within the effective period
            if (!EffectivePeriod.Contains(DateTime.Today))
                throw new Exception("Stock not currently active");

            UpdatePrice(DateTime.Today, currentPrice);
        }

        private void UpdatePrice(DateTime date, decimal price)
        {
            if (_Prices.ContainsKey(date))
                _Prices[date] = price;
            else
                _Prices.Add(date, price);
        }

        public decimal GetPrice(DateTime date)
        {
            if (_Prices.ContainsKey(date))
                return _Prices[date];
            else
                return ClosestPrice(date);
        }

        private decimal ClosestPrice(DateTime date)
        {
            if (_Prices.Keys.Count == 0)
                return 0.00m;

            int begin = 0;
            int end = _Prices.Keys.Count;
            while (end > begin)
            {
                int index = (begin + end) / 2;
                var el = _Prices.Keys[index];
                if (el.CompareTo(date) >= 0)
                    end = index;
                else
                    begin = index + 1;
            }

            return _Prices.Values[end - 1];
        } 

        public void ChangeProperties(DateTime changeDate, string newAsxCode, string newName, AssetCategory newAssetCategory)
        {           
            var properties = Properties[changeDate];

            var @event = new StockPropertiesChangedEvent(Id,
                changeDate,
                newAsxCode,
                newName,
                newAssetCategory);

            Apply(@event);
            _EventStore.StoreEvent(StockRepository.StreamId, @event, Version);
        }

        public void ChangeDRPRules(DateTime changeDate, bool drpActive, RoundingRule newDividendRoundingRule, DRPMethod newDrpMethod)
        {
            var properties = Properties[changeDate];

            var @event = new ChangeDividendReinvestmentPlanEvent(Id,
                changeDate,
                drpActive,
                newDividendRoundingRule,
                newDrpMethod);

            Apply(@event);
            _EventStore.StoreEvent(StockRepository.StreamId, @event, Version);
        }

        public void Apply(StockPropertiesChangedEvent @event)
        {
            Version++;

            var newProperties = new StockProperties(
                @event.ASXCode,
                @event.Name,
                @event.Category);

            Properties.Change(@event.ChangeDate, newProperties);
        }

        public void Apply(ChangeDividendReinvestmentPlanEvent @event)
        {
            Version++;

            var newProperties = new DividendReinvestmentPlan(
                @event.DRPActive,
                @event.DividendRoundingRule,
                @event.DRPMethod);

            DividendReinvestmentPlan.Change(@event.ChangeDate, newProperties);
        }
    }

    public struct StockProperties
    {
        public readonly string ASXCode;
        public readonly string Name;
        public readonly AssetCategory Category;

        public StockProperties(string asxCode, string name, AssetCategory category)
        {
            ASXCode = asxCode;
            Name = name;
            Category = category;
        }
    }

    public struct DividendReinvestmentPlan
    {
        public readonly bool DRPActive;
        public readonly RoundingRule DividendRoundingRule;
        public readonly DRPMethod DRPMethod;

        public DividendReinvestmentPlan(bool drpActive, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
        {
            DRPActive = drpActive;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }
    }


}
