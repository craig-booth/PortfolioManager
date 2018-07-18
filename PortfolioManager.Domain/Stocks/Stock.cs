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

    public class Stock : EffectiveEntity
    {
        public int Version { get; protected set; } = 0;
        protected IEventStream _EventStream;

        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();
        
        public bool Trust { get; private set; }
        public EffectiveProperties<StockProperties> Properties { get; } = new EffectiveProperties<StockProperties>();
        public EffectiveProperties<DividendRules> DividendRules { get; } = new EffectiveProperties<DividendRules>();

        private readonly Dictionary<Guid, CorporateAction> _CorporateActions = new Dictionary<Guid, CorporateAction>();

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

            var dividendRules = new DividendRules(RoundingRule.Round, false, DRPMethod.Round);
            DividendRules.Change(@event.ListingDate, dividendRules);
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
            DividendRules.End(@event.DelistedDate);

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

        public void ChangeDividendRules(DateTime changeDate, RoundingRule newDividendRoundingRule, bool drpActive, DRPMethod newDrpMethod)
        {
            var properties = Properties[changeDate];

            var @event = new ChangeDividendRulesEvent(Id, Version, changeDate, newDividendRoundingRule, drpActive, newDrpMethod);

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

        public void Apply(ChangeDividendRulesEvent @event)
        {
            Version++;

            var newProperties = new DividendRules(
                @event.DividendRoundingRule,
                @event.DRPActive,
                @event.DRPMethod);

            DividendRules.Change(@event.ChangeDate, newProperties);
        }

        public CorporateAction CorporateAction(Guid id)
        {
            if (_CorporateActions.ContainsKey(id))
                return _CorporateActions[id];
            else
                return null;
        }

        public T CorporateAction<T>(Guid id) where T : CorporateAction
        {
            if (_CorporateActions.ContainsKey(id))
                return _CorporateActions[id] as T;
            else
                return null;
        }

        public IEnumerable<CorporateAction> CorporateActions()
        {
            return _CorporateActions.Values;
        }

        public IEnumerable<CorporateAction> CorporateActions(DateRange dateRange)
        {
            return _CorporateActions.Values.Where(x => dateRange.Contains(x.ActionDate));
        }

        public IEnumerable<T> CorporateActions<T>() where T : CorporateAction
        {
            return _CorporateActions.Values.Where(x => x is T).Select(x => x as T);
        }

        public IEnumerable<T> CorporateActions<T>(DateRange dateRange) where T : CorporateAction
        {
            return _CorporateActions.Values.Where(x => x is T && dateRange.Contains(x.ActionDate)).Select(x => x as T);
        }

        public void AddCapitalReturn(Guid id, DateTime recordDate, string description, DateTime paymentDate, decimal amount)
        {
            if (description == "")
                description = "Capital Return " + amount.ToString("$#,##0.00###");

            var @event = new CapitalReturnAddedEvent(Id, Version, id, recordDate, description, paymentDate, amount);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(CapitalReturnAddedEvent @event)
        {
            Version++;

            var capitalReturn = new CapitalReturn(@event.ActionId, this, @event.ActionDate, @event.Description, @event.PaymentDate, @event.Amount);

            _CorporateActions.Add(capitalReturn.Id, capitalReturn);
        }

        public void AddDividend(Guid id, DateTime recordDate, string description, DateTime paymentDate, decimal dividendAmount, decimal companyTaxRate, decimal percentFranked, decimal drpPrice)
        {
            if (description == "")
                description = "Dividend " + MathUtils.FormatCurrency(dividendAmount, false, true);

            var @event = new DividendAddedEvent(Id, Version, id, recordDate, description, paymentDate, dividendAmount, companyTaxRate, percentFranked, drpPrice);

            Apply(@event);
            _EventStream.StoreEvent(@event);
        }

        public void Apply(DividendAddedEvent @event)
        {
            Version++;

            var dividend = new Dividend(@event.ActionId, this, @event.ActionDate, @event.Description, @event.PaymentDate, @event.DividendAmount, @event.CompanyTaxRate, @event.PercentFranked, @event.DRPPrice);

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

    public struct DividendRules
    {
        public readonly RoundingRule DividendRoundingRule;

        public readonly bool DRPActive;       
        public readonly DRPMethod DRPMethod;

        public DividendRules(RoundingRule dividendRoundingRule, bool drpActive, DRPMethod drpMethod)
        {
            DividendRoundingRule = dividendRoundingRule;
            DRPActive = drpActive;
            DRPMethod = drpMethod;
        }
    }


}
