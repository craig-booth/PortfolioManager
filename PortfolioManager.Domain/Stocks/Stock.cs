using System;
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

    public class Stock : EffectiveEntity, ITrackedEntity
    {
        public int Version { get; protected set; } = 0;
        private EventList _Events = new EventList();

        private StockPriceHistory _Prices = new StockPriceHistory();

        public bool Trust { get; private set; }

        protected EffectiveProperties<StockProperties> _Properties { get; } = new EffectiveProperties<StockProperties>();
        public IEffectiveProperties<StockProperties> Properties => _Properties;

        protected EffectiveProperties<DividendRules> _DividendRules { get; } = new EffectiveProperties<DividendRules>();
        public IEffectiveProperties<DividendRules> DividendRules => _DividendRules;

        public CorporateActionCollection CorporateActions { get; }

        public Stock()
        {
            CorporateActions = new CorporateActionCollection(this, this._Events);
        }

        public override string ToString()
        {
            var properties = Properties.ClosestTo(DateTime.Today);
            return String.Format("{0} - {1}", properties.ASXCode, properties.Name);
        }

        protected void PublishEvent(Event @event)
        {
            _Events.Add(@event);
        }

        public void List(string asxCode, string name, bool trust, AssetCategory category)
        {
            var @event = new StockListedEvent(Id, Version, asxCode, name, EffectivePeriod.FromDate, category, trust);
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(StockListedEvent @event)
        {
            Version++;
            Trust = @event.Trust;

            Start(@event.EntityId, @event.ListingDate);

            var properties = new StockProperties(@event.ASXCode, @event.Name, @event.Category);
            _Properties.Change(@event.ListingDate, properties);

            var dividendRules = new DividendRules(0.30m, RoundingRule.Round, false, DRPMethod.Round);
            _DividendRules.Change(@event.ListingDate, dividendRules);
        }

        public void DeList(DateTime date)
        {
            var @event = new StockDelistedEvent(Id, Version, date);
            Apply(@event);

            PublishEvent(@event);
        }

        public virtual void Apply(StockDelistedEvent @event)
        {
            Version++;

            _Properties.End(@event.DelistedDate);
            _DividendRules.End(@event.DelistedDate);

            End(@event.DelistedDate);
        }

        public virtual void Apply(CorporateActionAddedEvent @event)
        {
            Version++;

            dynamic dynamicEvent = @event;
            CorporateActions.Apply(dynamicEvent);
        }

        public void UpdateClosingPrice(DateTime date, decimal closingPrice)
        {
            // Check that the date is within the effective period
            if (!EffectivePeriod.Contains(date))
                throw new Exception(String.Format("Stock not active on {0}", date));

            var @event = new ClosingPricesAddedEvent(Id, Version, new ClosingPricesAddedEvent.ClosingPrice[] { new ClosingPricesAddedEvent.ClosingPrice(date, closingPrice) });
            Apply(@event);

            PublishEvent(@event);
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

            PublishEvent(@event);
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
            _Prices.UpdatePrice(date, price);
        }

        public decimal GetPrice(DateTime date)
        {
            return _Prices.GetPrice(date);
        }

        public IEnumerable<KeyValuePair<DateTime, decimal>> GetPrices(DateRange dateRange)
        {
            return _Prices.GetPrices(dateRange);
        }

   
        public DateTime DateOfLastestPrice()
        {
            return _Prices.LatestDate;
        }

        public void ChangeProperties(DateTime changeDate, string newAsxCode, string newName, AssetCategory newAssetCategory)
        {           
            var properties = Properties[changeDate];

            var @event = new StockPropertiesChangedEvent(Id, Version, changeDate, newAsxCode, newName, newAssetCategory);

            Apply(@event);
            _Events.Add(@event);
        }

        public void ChangeDividendRules(DateTime changeDate, decimal companyTaxRate, RoundingRule newDividendRoundingRule, bool drpActive, DRPMethod newDrpMethod)
        {
            var properties = Properties[changeDate];

            var @event = new ChangeDividendRulesEvent(Id, Version, changeDate, companyTaxRate, newDividendRoundingRule, drpActive, newDrpMethod);

            Apply(@event);
            _Events.Add(@event);
        }

        public void Apply(StockPropertiesChangedEvent @event)
        {
            Version++;

            var newProperties = new StockProperties(
                @event.ASXCode,
                @event.Name,
                @event.Category);

            _Properties.Change(@event.ChangeDate, newProperties);
        }

        public void Apply(ChangeDividendRulesEvent @event)
        {
            Version++;

            var newProperties = new DividendRules(
                @event.CompanyTaxRate,
                @event.DividendRoundingRule,
                @event.DRPActive,
                @event.DRPMethod);

            _DividendRules.Change(@event.ChangeDate, newProperties);
        }

        public IEnumerable<Event> FetchEvents()
        {
            return _Events.Fetch();
        }

        public void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;
                Apply(dynamicEvent);
            }              
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

    public struct DividendRules
    {
        public readonly decimal CompanyTaxRate;
        public readonly RoundingRule DividendRoundingRule;

        public readonly bool DRPActive;       
        public readonly DRPMethod DRPMethod;

        public DividendRules(decimal companyTaxRate, RoundingRule dividendRoundingRule, bool drpActive, DRPMethod drpMethod)
        {
            CompanyTaxRate = companyTaxRate;
            DividendRoundingRule = dividendRoundingRule;
            DRPActive = drpActive;
            DRPMethod = drpMethod;
        }
    }


}
