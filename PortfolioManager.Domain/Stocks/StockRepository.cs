using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
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

        void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category);
        void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities);
        void DelistStock(Guid id, DateTime date);
    }

    public class StockRepository : IStockRepository
    {
        private IEventStream _EventStream;

        private Dictionary<Guid, Stock> _Stocks = new Dictionary<Guid, Stock>();

        public StockRepository(IEventStream eventStream)
        {
            _EventStream = eventStream;
        }

        public void LoadFromEventStream()
        {
            var events = _EventStream.RetrieveEvents();
            foreach (var @event in events)
            {
                if (@event is StockListedEvent)
                {
                    var listingEvent = @event as StockListedEvent;
                    var stock = new Stock(listingEvent.EntityId, listingEvent.ListingDate, _EventStream);
                    _Stocks.Add(@event.EntityId, stock);

                    stock.Apply(listingEvent);
                }
                else if (@event is StapledSecurityListedEvent)
                {
                    var listingEvent = @event as StapledSecurityListedEvent;
                    var stock = new StapledSecurity(listingEvent.EntityId, listingEvent.ListingDate, _EventStream);
                    _Stocks.Add(@event.EntityId, stock);

                    stock.Apply(listingEvent);
                }
                else
                {
                    var stock = _Stocks[@event.EntityId];
                    dynamic dynamicEvent = @event;

                    if (stock is StapledSecurity)
                        (stock as StapledSecurity).Apply(dynamicEvent);
                    else
                        stock.Apply(dynamicEvent);
                }
            }
        }

        public Stock Get(Guid id)
        {
            if (_Stocks.ContainsKey(id))
                return _Stocks[id];
            else
                return null;
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

        public void ListStock(Guid id, string asxCode, string name, DateTime listingDate, bool trust, AssetCategory category)
        {
            // Check that id is unique
            if (_Stocks.ContainsKey(id))
                throw new Exception("Id not unique");

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            var stock = new Stock(id, listingDate, _EventStream);
            _Stocks.Add(stock.Id, stock);

            stock.List(asxCode, name, trust, category);
        }

        public void ListStapledSecurity(Guid id, string asxCode, string name, DateTime listingDate, AssetCategory category, IEnumerable<StapledSecurityChild> childSecurities)
        {   
            // Check that id is unique
            if (_Stocks.ContainsKey(id))
                throw new Exception("Id not unique");

            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            var stapledSecurity = new StapledSecurity(id, listingDate, _EventStream);
            _Stocks.Add(stapledSecurity.Id, stapledSecurity);

            stapledSecurity.List(asxCode, name, category, childSecurities);
        }

        public void DelistStock(Guid id, DateTime date)
        {
            // Check that stock exists
            var stock = Get(id);
            if (stock == null)
                throw new Exception("Stock not found");

            stock.DeList(date);
        }
    }
}
