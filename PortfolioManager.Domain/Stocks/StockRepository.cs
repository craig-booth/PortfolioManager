using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public interface IStockRepository
    {
        Stock Get(Guid id);
        Stock Get(string asxCode, DateTime date);
        IEnumerable<Stock> All();
        IEnumerable<Stock> All(DateTime date);
        IEnumerable<Stock> All(DateRange dateRange);
        IEnumerable<Stock> Find(DateTime date, Func<StockProperties, bool> predicate);
        IEnumerable<Stock> Find(DateRange dateRange, Func<StockProperties, bool> predicate);
    }

    public class StockRepository : IStockRepository
    {
        public static readonly Guid StreamId = new Guid("2FAD2856-9675-4F73-81F0-A12C60E3A9CB");
        private IEventStore _EventStore;

        private Dictionary<Guid, Stock> _Stocks = new Dictionary<Guid, Stock>();

        public StockRepository(IEventStore eventStore)
        {
            _EventStore = eventStore;
        }

        public Stock Get(Guid id)
        {
            return _Stocks[id];
        }

        public Stock Get(string asxCode, DateTime date)
        {
            return _Stocks.Values.FirstOrDefault(x => x.IsEffectiveAt(date) && x.Properties.Matches(date, y => y.ASXCode == asxCode));
        }

        public IEnumerable<Stock> All()
        {
            return _Stocks.Values;
        }

        public IEnumerable<Stock> All(DateTime date)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveAt(date));
        }

        public IEnumerable<Stock> All(DateRange dateRange)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveDuring(dateRange));
        }

        public IEnumerable<Stock> Find(DateTime date, Func<StockProperties, bool> predicate)
        {
            return All(date).Where(x => x.Properties.Matches(date, predicate));
        }

        public IEnumerable<Stock> Find(DateRange dateRange, Func<StockProperties, bool> predicate)
        {
            return All(dateRange).Where(x => x.Properties.Matches(dateRange, predicate));
        }

        public void ListStock(string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category)
        {
            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            var stock = new Stock(Guid.NewGuid(), listingDate, _EventStore);
            _Stocks.Add(stock.Id, stock);

            stock.List(asxCode, name, trust, category);
        }

        public void ListStapledSecurity(string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities)
        {
            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            var stapledSecurity = new StapledSecurity(Guid.NewGuid(), listingDate, _EventStore);
            _Stocks.Add(stapledSecurity.Id, stapledSecurity);

            stapledSecurity.List(asxCode, name, category, childSecurities);
        }

        public void DelistStock(string asxCode, DateTime date)
        {
            // Check that stock exists
            var stock = Get(asxCode, date);
            if (stock == null)
                throw new Exception("Stock not found");

            stock.DeList(date);
        }

        public void Apply(IEvent @event)
        {

            if (@event is StockListedEvent)
            {
                var listingEvent = @event as StockListedEvent;
                var stock = new Stock(listingEvent.Id, listingEvent.ListingDate, _EventStore);
                _Stocks.Add(@event.Id, stock);

                stock.Apply(listingEvent);
            }
            else if (@event is StapledSecurityListedEvent)
            {
                var listingEvent = @event as StapledSecurityListedEvent;
                var stock = new StapledSecurity(listingEvent.Id, listingEvent.ListingDate, _EventStore);
                _Stocks.Add(@event.Id, stock);

                stock.Apply(listingEvent);
            }
            else
            {
                var stock = _Stocks[@event.Id];
                dynamic dynamicEvent = @event;

                if (stock is StapledSecurity)
                    (stock as StapledSecurity).Apply(dynamicEvent);
                else
                    stock.Apply(dynamicEvent);
            }
        }
    }
}
