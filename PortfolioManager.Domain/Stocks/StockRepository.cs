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
            return _Stocks.Values.Where(x => x.IsEffectiveAt(date) && x.Properties.Matches(date, predicate));
        }

        public IEnumerable<Stock> Find(DateRange dateRange, Func<StockProperties, bool> predicate)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveDuring(dateRange) && x.Properties.Matches(dateRange, predicate));
        }

        public void ListStock(string asxCode, string name, DateTime listingDate, StockType type, AssetCategory category)
        {
            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));
           
            var @event = new StockListedEvent(Guid.NewGuid(), asxCode, name, listingDate, type, category);
            Apply(@event);

            _EventStore.StoreEvent(@event);
        }

        public void Apply(StockListedEvent @event)
        {
            var stock = new Stock(@event.Id, @event.ListingDate, _EventStore);
            stock.Apply(@event);
        
            _Stocks.Add(@event.Id, stock);
        }

        public void DelistStock(string asxCode, DateTime date)
        {
            // Check that stock exists
            var stock = Get(asxCode, date);
            if (stock == null)
                throw new Exception("Stock not found");

            var @event = new StockDelistedEvent(stock.Id, date);
            Apply(@event);

            _EventStore.StoreEvent(@event);
        }

        public void Apply(StockDelistedEvent @event)
        {
            var stock = _Stocks[@event.Id];
            stock.End(@event.DelistedDate);
        }
    }
}
