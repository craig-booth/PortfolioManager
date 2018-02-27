using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public interface IStock : IEffectiveEntity<StockProperties>
    {
        decimal GetPrice(DateTime date);
    }

    public class Stock : EffectiveEntity<StockProperties>, IStock
    {
        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();

        private IEventStore _EventStore;

        public Stock(Guid id, DateTime listingDate, IEventStore eventStore)
            : base(id, listingDate)
        {
            _EventStore = eventStore;
        }

        public void Apply(StockListedEvent @event)
        {
            _CurrentProperties = new StockProperties(Id, @event.ListingDate, DateUtils.NoEndDate, @event.ASXCode, @event.Name, @event.Type, @event.Category, @event.DividendRoundingRule, @event.DRPMethod);
            _Properties.Add(_CurrentProperties);
        }

        public void UpdateClosingPrice(DateTime date, decimal closingPrice)
        {
            // Check that the date is within the effective period
            if (! EffectivePeriod.Contains(date))
                throw new Exception(String.Format("Stock not active on {0}", date));

            var @event = new ClosingPriceAddedEvent(Id, date, closingPrice);
            Apply(@event);

            _EventStore.StoreEvent(Id, @event);
        }

        public void Apply(ClosingPriceAddedEvent @event)
        {
            UpdatePrice(@event.Date, @event.ClosingPrice);
        }

        public void UpdateCurrentPrice(decimal currentPrice)
        {
            // Check that the date is within the effective period
            if (!EffectivePeriod.Contains(DateTime.Today))
                throw new Exception("Stock not currently active ");

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

        public void ChangeName(DateTime changeDate, string newAsxCode, string newName)
        {

        }

        public void ChangeCategory(DateTime changeDate, AssetCategory assetCategory)
        {

        }

        public void ChangeDRPRules(DateTime changeDate, bool DRPActive, RoundingRule newDividendRoundingRule, DRPMethod newDrpMethod)
        {

        }

        public void Apply(StockPropertiesChangedEvent @event)
        {
      /*      Change(@event.ChangeDate, x => {
                x.Name = @event.Name;
                x.ListingDate = @event.ChangeDate;
        public StockType Type { get; }
        public AssetCategory Category { get; }
        public bool DRPActive { get; }
        public RoundingRule DividendRoundingRule { get; }
        public DRPMethod DRPMethod { get; }) */
        }
    }

    public class StockProperties : EffectiveProperties
    {
        public string ASXCode { get; }
        public string Name { get; }
        public StockType Type { get; }
        public AssetCategory Category { get; }
        public bool DRPActive { get; }
        public RoundingRule DividendRoundingRule { get; }
        public DRPMethod DRPMethod { get; }

        public StockProperties(Guid id, DateTime fromDate, string asxCode, string name, StockType type, AssetCategory category, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
            : base(id, fromDate)
        {
            ASXCode = asxCode;
            Name = name;
            Type = type;
            Category = category;
            DRPActive = false;
            DividendRoundingRule = dividendRoundingRule;
            DRPMethod = drpMethod;
        }

        public override EffectiveProperties Copy(DateRange newRange)
        {
            var newStockProperties = new StockProperties(Id, newRange.FromDate, newRange.ToDate, ASXCode, Name, Type, Category, DividendRoundingRule, DRPMethod);

            return newStockProperties;
        }
    }
}
