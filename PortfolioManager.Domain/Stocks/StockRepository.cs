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
        public int Version { get; private set; } = 0;
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
            return _Stocks.Values.Where(x => x.Parent.Matches(y => y == Guid.Empty));
        }

        public IEnumerable<Stock> All(DateTime date)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveAt(date) && x.Parent.Matches(date, y => y == Guid.Empty));
        }

        public IEnumerable<Stock> All(DateRange dateRange)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveDuring(dateRange) && x.Parent.Matches(dateRange, y => y == Guid.Empty));
        }

        public IEnumerable<Stock> Find(DateTime date, Func<StockProperties, bool> predicate)
        {
            return All(date).Where(x => x.Properties.Matches(date, predicate));
        }

        public IEnumerable<Stock> Find(DateRange dateRange, Func<StockProperties, bool> predicate)
        {
            return All(dateRange).Where(x => x.Properties.Matches(dateRange, predicate));
        }

        public void ListStock(string asxCode, string name, DateTime listingDate, StockType type, AssetCategory category, IEnumerable<Guid> childSecurities)
        {
            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Properties.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the code {0} at {1}", asxCode, listingDate));

            // Check that type is StapledSecurity if adding child stocks
            if (type == StockType.StapledSecurity)
            {
                if ((childSecurities == null) || ! childSecurities.Any())
                    throw new Exception("Stapled Security must have child securities");

                foreach (var stockId in childSecurities)
                {
                    if (! _Stocks.ContainsKey(stockId))
                        throw new Exception(String.Format("Stock {0} not found", stockId));
                }
            }
            else
            {
                if ((childSecurities != null) && childSecurities.Any())
                    throw new Exception("Child securities can only be added to Stapled Securities");
            }

            var @event = new StockListedEvent(Guid.NewGuid(), asxCode, name, listingDate, category, type, childSecurities?.ToArray());
            Apply(@event);

            _EventStore.StoreEvent(StreamId, @event, Version);
        }

        public void Apply(StockListedEvent @event)
        {
            Version++;

            var stock = new Stock(@event.Id, @event.ListingDate, _EventStore);
            stock.Apply(@event);
            _Stocks.Add(@event.Id, stock);

            if (@event.Type == StockType.StapledSecurity)
            {
                var childSecurities = new List<Stock>();
                foreach (var stockId in @event.ChildSecurities)
                {
                    var childStock = _Stocks[stockId];
                    childSecurities.Add(childStock);

                    childStock.SetParentStock(@event.ListingDate, stock);
                }
                stock.AddChildSecurties(childSecurities);
            }            
        }

        public void DelistStock(string asxCode, DateTime date)
        {
            // Check that stock exists
            var stock = Get(asxCode, date);
            if (stock == null)
                throw new Exception("Stock not found");

            var @event = new StockDelistedEvent(stock.Id, date);
            Apply(@event);

            _EventStore.StoreEvent(StreamId, @event, Version);
        }

        public void Apply(StockDelistedEvent @event)
        {
            Version++;

            var stock = _Stocks[@event.Id];
            stock.End(@event.DelistedDate);
        }
    }
}
