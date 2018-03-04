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
        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();
        private IEventStore _EventStore;

        public StockType Type { get; private set; }
        public EffectiveProperties<StockProperties> Properties { get; } = new EffectiveProperties<StockProperties>();
        public EffectiveProperties<DividendReinvestmentPlan> DividendReinvestmentPlan { get; } = new EffectiveProperties<DividendReinvestmentPlan>();
        public EffectiveProperties<ParentStock> Parent { get; } = new EffectiveProperties<ParentStock>();

        private List<Stock> _ChildSecurities = new List<Stock>();
        public IReadOnlyList<Stock> ChildSecurities
        {
            get { return _ChildSecurities; }
        }

        public Stock(Guid id, DateTime listingDate, IEventStore eventStore)
            : base(id, listingDate)
        {
            _EventStore = eventStore;
        }

        public void Apply(StockListedEvent @event)
        {
            Type = @event.Type;

            var properties = new StockProperties(@event.ASXCode, @event.Name, @event.Category);
            Properties.Change(@event.ListingDate, properties);

            var drp = new DividendReinvestmentPlan(false, RoundingRule.Round, DRPMethod.Round);
            DividendReinvestmentPlan.Change(@event.ListingDate, drp);

            Parent.Change(@event.ListingDate, new ParentStock(null));
        }

        public void AddChildSecurties(IEnumerable<Stock> childSecurities)
        {
            _ChildSecurities.AddRange(childSecurities);
        }

        public void SetParentStock(DateTime date, Stock parent)
        {
            Parent.Change(date, new ParentStock(parent));
        }

        public void UpdateClosingPrice(DateTime date, decimal closingPrice)
        {
            // Check that the date is within the effective period
            if (! EffectivePeriod.Contains(date))
                throw new Exception(String.Format("Stock not active on {0}", date));

            var @event = new ClosingPriceAddedEvent(Id, date, closingPrice);
            Apply(@event);

            _EventStore.StoreEvent(@event);
        }

        public void Apply(ClosingPriceAddedEvent @event)
        {
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
            _EventStore.StoreEvent(@event);
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
            _EventStore.StoreEvent(@event);
        }

        public void Apply(StockPropertiesChangedEvent @event)
        {
            var newProperties = new StockProperties(
                @event.ASXCode,
                @event.Name,
                @event.Category);

            Properties.Change(@event.ChangeDate, newProperties);
        }

        public void Apply(ChangeDividendReinvestmentPlanEvent @event)
        {
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

    public struct ParentStock
    {
        public readonly Stock Parent;

        public ParentStock(Stock parent)
        {
            Parent = parent;
        }
    }

}
