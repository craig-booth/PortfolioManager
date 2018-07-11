﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.Domain.CorporateActions;
using PortfolioManager.Domain.CorporateActions.Events;

namespace PortfolioManager.Domain.Stocks
{

    public class Stock : EffectiveEntity
    {
        public int Version { get; protected set; } = 0;
        protected IEventStream _EventStream;

        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();
        
        public bool Trust { get; private set; }
        public EffectiveProperties<StockProperties> Properties { get; } = new EffectiveProperties<StockProperties>();
        public EffectiveProperties<DividendReinvestmentPlan> DividendReinvestmentPlan { get; } = new EffectiveProperties<DividendReinvestmentPlan>();

        private readonly Dictionary<Guid, ICorporateAction> _CorporateActions = new Dictionary<Guid, ICorporateAction>();
        public IReadOnlyDictionary<Guid, ICorporateAction> CorporateActions
        {
            get { return _CorporateActions; }
        }

        public Stock(Guid id, DateTime listingDate, IEventStream eventStream)
            : base(id, listingDate)
        {
            _EventStream = eventStream;
        }

        public override string ToString()
        {
            var properties = Properties.ClosestTo(DateTime.Today);
            return String.Format("{0} - {1}", properties.ASXCode, properties.Name);
        }

        public void List(string asxCode, string name, bool trust, AssetCategory category)
        {
            var @event = new StockListedEvent(Id, Version, asxCode, name, EffectivePeriod.FromDate, category, trust);
            Apply(@event);

            _EventStream.StoreEvent(@event);
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
            var @event = new StockDelistedEvent(Id, Version, date);
            Apply(@event);

            _EventStream.StoreEvent(@event);
        }

        public virtual void Apply(StockDelistedEvent @event)
        {
            Version++;

            Properties.End(@event.DelistedDate);
            DividendReinvestmentPlan.End(@event.DelistedDate);

            End(@event.DelistedDate);
        }

        public void UpdateClosingPrice(DateTime date, decimal closingPrice)
        {
            // Check that the date is within the effective period
            if (!EffectivePeriod.Contains(date))
                throw new Exception(String.Format("Stock not active on {0}", date));

            var @event = new ClosingPricesAddedEvent(Id, Version, new ClosingPricesAddedEvent.ClosingPrice[] { new ClosingPricesAddedEvent.ClosingPrice(date, closingPrice) });
            Apply(@event);

            _EventStream.StoreEvent(@event);
        }

        public void UpdateClosingPrices(IEnumerable<Tuple<DateTime, decimal>> closingPrices)
        {
            // Check that the date is within the effective period
            foreach (var closingPrice in closingPrices)
            {
                if (!EffectivePeriod.Contains(closingPrice.Item1))
                    throw new Exception(String.Format("Stock not active on {0}", closingPrice.Item1));
            }

            var @event = new ClosingPricesAddedEvent(Id, Version, closingPrices.Select(x => new ClosingPricesAddedEvent.ClosingPrice(x.Item1, x.Item2)));
            Apply(@event);

            _EventStream.StoreEvent(@event);
        }

        public void Apply(ClosingPricesAddedEvent @event)
        {
            Version++;
            foreach(var closingPrice in @event.ClosingPrices)
                UpdatePrice(closingPrice.Date, closingPrice.Price);
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

        public IEnumerable<KeyValuePair<DateTime, decimal>> GetPrices(DateRange dateRange)
        {
            return _Prices.Where(x => dateRange.Contains(x.Key)).AsEnumerable();
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

        public DateTime DateOfLastestPrice()
        {
            var latestPrice = _Prices.LastOrDefault(x => x.Key != DateTime.Today);

            if (latestPrice.Key != null)
                return latestPrice.Key;
            else
                return DateUtils.NoDate;
        }

        public void ChangeProperties(DateTime changeDate, string newAsxCode, string newName, AssetCategory newAssetCategory)
        {           
            var properties = Properties[changeDate];

            var @event = new StockPropertiesChangedEvent(Id, Version, changeDate, newAsxCode, newName, newAssetCategory);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void ChangeDRPRules(DateTime changeDate, bool drpActive, RoundingRule newDividendRoundingRule, DRPMethod newDrpMethod)
        {
            var properties = Properties[changeDate];

            var @event = new ChangeDividendReinvestmentPlanEvent(Id, Version, changeDate,  drpActive, newDividendRoundingRule, newDrpMethod);

            Apply(@event);
            _EventStream.StoreEvent(@event);
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

        public void AddCapitalReturn(DateTime recordDate, string description, DateTime paymentDate, decimal amount)
        {
             var @event = new CapitalReturnAddedEvent(Id, Version, Guid.NewGuid(), recordDate, description, paymentDate, amount);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(CapitalReturnAddedEvent @event)
        {
            Version++;

            var capitalReturn = new CapitalReturn(this, @event.ActionId, @event.ActionDate, @event.Description, @event.PaymentDate, @event.Amount);

            _CorporateActions.Add(capitalReturn.Id, capitalReturn);
        }

        public void AddDividend(DateTime recordDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            var @event = new DividendAddedEvent(Id, Version, Guid.NewGuid(), recordDate, description, paymentDate, dividendAmount, companyTaxRate, percentFranked, drpPrice);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(DividendAddedEvent @event)
        {
            Version++;

            var dividend = new Dividend(this, @event.ActionId, @event.ActionDate, @event.Description, @event.PaymentDate, @event.DividendAmount, @event.CompanyTaxRate, @event.PercentFranked, @event.DRPPrice);

            _CorporateActions.Add(dividend.Id, dividend);
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
