﻿using System;
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

        public EffectiveProperties<StockProperties> Properties { get; } = new EffectiveProperties<StockProperties>();
        public EffectiveProperties<DividendReinvestmentPlan> DividendReinvestmentPlan { get; } = new EffectiveProperties<DividendReinvestmentPlan>();

        public Stock(Guid id, DateTime listingDate, IEventStore eventStore)
            : base(id, listingDate)
        {
            _EventStore = eventStore;
        }

        public void Apply(StockListedEvent @event)
        {
            var properties = new StockProperties(@event.ASXCode, @event.Name, @event.Type, @event.Category);
            Properties.Change(@event.ListingDate, properties);

            var drp = new DividendReinvestmentPlan(false, RoundingRule.Round, DRPMethod.Round);
            DividendReinvestmentPlan.Change(@event.ListingDate, drp);
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
            var properties = Properties.Get(changeDate);

            var @event = new StockPropertiesChangedEvent(Id,
                changeDate,
                newAsxCode,
                newName,
                properties.Type,
                newAssetCategory);

            Apply(@event);
            _EventStore.StoreEvent(@event);
        }

        public void ChangeDRPRules(DateTime changeDate, bool drpActive, RoundingRule newDividendRoundingRule, DRPMethod newDrpMethod)
        {
            var properties = Properties.Get(changeDate);

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
                @event.Type,
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
        public readonly StockType Type;
        public readonly AssetCategory Category;

        public StockProperties(string asxCode, string name, StockType type, AssetCategory category)
        {
            ASXCode = asxCode;
            Name = name;
            Type = type;
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
