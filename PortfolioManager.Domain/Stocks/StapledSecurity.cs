﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public class StapledSecurity : Stock
    {
        private StapledSecurityChild[] _ChildSecurities;
        public IReadOnlyList<StapledSecurityChild> ChildSecurities
        {
            get { return _ChildSecurities; }
        }

        private EffectiveProperties<RelativeNTA> _RelativeNTAs = new EffectiveProperties<RelativeNTA>();
        public IEffectiveProperties<RelativeNTA> RelativeNTAs => _RelativeNTAs;

        public StapledSecurity(IStockPriceHistory stockPriceHistory)
            : base(stockPriceHistory)
        {

        }

        public void List(string asxCode, string name, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities)
        {
            var @event = new StapledSecurityListedEvent(Id, Version, asxCode, name, EffectivePeriod.FromDate, category, childSecurities?.ToArray());
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(StapledSecurityListedEvent @event)
        {
            Version++;

            Start(@event.EntityId, @event.ListingDate);

            var properties = new StockProperties(@event.ASXCode, @event.Name, @event.Category);
            _Properties.Change(@event.ListingDate, properties);

            _ChildSecurities = new StapledSecurityChild[@event.ChildSecurities.Length];
            for (var i = 0; i < @event.ChildSecurities.Length; i++)
                _ChildSecurities[i] = new StapledSecurityChild(@event.ChildSecurities[i].ASXCode, @event.ChildSecurities[i].Name, @event.ChildSecurities[i].Trust);
            
            var dividendRules = new DividendRules(0.30m, RoundingRule.Round, false, DRPMethod.Round);
            _DividendRules.Change(@event.ListingDate, dividendRules);

            var percentages = new decimal[_ChildSecurities.Length];
            for (var i = 0; i < @event.ChildSecurities.Length; i++)
                percentages[i] = 1 / _ChildSecurities.Length;

            _RelativeNTAs.Change(@event.ListingDate, new RelativeNTA(percentages));
        }

        public override void Apply(StockDelistedEvent @event)
        {
            base.Apply(@event);

            _RelativeNTAs.End(@event.DelistedDate);
        }

        public void SetRelativeNTAs(DateTime date, IEnumerable<decimal> percentages)
        {
            var percentagesArray = percentages.ToArray();

            if (percentagesArray.Length != _ChildSecurities.Length)
                throw new Exception(String.Format("Expecting {0} values but received {1}", _ChildSecurities.Length, percentagesArray.Length));

            var total = percentagesArray.Sum();
            if (total != 1.00m)
                throw new Exception(String.Format("Total percentage must add up to 1.00 but was {0}", total));

            var @event = new RelativeNTAChangedEvent(Id, Version, date, percentagesArray);
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(RelativeNTAChangedEvent @event)
        {
            Version++;

            _RelativeNTAs.Change(@event.Date, new RelativeNTA(@event.Percentages));
        }

        public new void ApplyEvents(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;
                Apply(dynamicEvent);
            }
        }

    }

    public class StapledSecurityChild
    {
        public string ASXCode { get; set; }
        public string Name { get; set; }
        public bool Trust { get; set; }

        public StapledSecurityChild(string asxCode, string name, bool trust)
        {
            ASXCode = asxCode;
            Name = name;
            Trust = trust;
        }
    }

    public struct RelativeNTA
    {
        public readonly decimal[] Percentages;

        public RelativeNTA(decimal[] percentages)
        {
            Percentages = percentages;
        }
    }
}
