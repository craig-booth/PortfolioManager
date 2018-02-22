using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public class StockExchange : 
        IEventHandler<StockListedEvent>,
        IEventHandler<StockDelistedEvent>,
        IEventHandler<NonTradingDayAddedEvent>,
        IEventHandler<ClosingPriceAddedEvent>
    {
        private IEventStore _EventStore;
        private Dictionary<Guid, Stock> _Stocks = new Dictionary<Guid, Stock>();

        private TradingCalander _TradingCalander = new TradingCalander();
        public ITradingCalander TradingCalander 
        {
            get
            {
                return _TradingCalander;
            }
        } 

        public StockExchange(IEventStore eventStore)
        {
            _EventStore = eventStore;
        }

        public Stock GetStock(Guid id)
        {
            return _Stocks[id];
        }

        public Stock GetStock(string asxCode, DateTime date)
        {
            return _Stocks.Values.FirstOrDefault(x => x.IsEffectiveAt(date) && x.Matches(y => y.ASXCode == asxCode));
        }

        public IEnumerable<Stock> GetStocks()
        {
            return _Stocks.Values;
        }

        public IEnumerable<Stock> GetStocks(DateTime date)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveAt(date));
        }

        public IEnumerable<Stock> GetStocks(DateTime date, Func<StockProperties, bool> predicate)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveAt(date) && x.Matches(predicate));
        }

        public IEnumerable<Stock> GetStocks(DateRange dateRange)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveDuring(dateRange));
        }

        public IEnumerable<Stock> GetStocks(DateRange dateRange, Func<StockProperties, bool> predicate)
        {
            return _Stocks.Values.Where(x => x.IsEffectiveDuring(dateRange) && x.Matches(predicate));
        }

        public void ListStock(string asxCode, string name, DateTime listingDate, StockType type, AssetCategory category, RoundingRule dividendRoundingRule, DRPMethod drpMethod)
        {
            // Check if stock already exists with this code
            var effectivePeriod = new DateRange(listingDate, DateUtils.NoEndDate);
            if (_Stocks.Values.Any(x => x.Matches(effectivePeriod, y => y.ASXCode == asxCode)))
                throw new Exception(String.Format("Stock already exists with the {0} at {1}", asxCode, listingDate));

            var @event = new StockListedEvent(asxCode, name, listingDate, type, category, dividendRoundingRule, drpMethod);
            ApplyEvent(@event);

            _EventStore.StoreEvent(Guid.Empty, @event);
        }

        public void ApplyEvent(StockListedEvent e)
        {
            var stock = new Stock(e.Id, e.ListingDate, e.ASXCode, e.Name, e.Type, e.Category, e.DividendRoundingRule, e.DRPMethod);
            _Stocks.Add(e.Id, stock);
        }

        public void DelistStock(string asxCode, DateTime date)
        {
            // Check that stock exists
            var stock = GetStock(asxCode, date);
            if (stock == null)
                throw new Exception("Stock not found");

            var @event = new StockDelistedEvent(stock.Id, date);
            ApplyEvent(@event);

            _EventStore.StoreEvent(Guid.Empty, @event);
        }

        public void ApplyEvent(StockDelistedEvent e)
        {
            var stock = _Stocks[e.Id];
            stock.End(e.DelistedDate);
        }

        public void AddNonTradingDay(DateTime date)
        {
            // Check that the non trading day is not already defined
            if (! _TradingCalander.IsTradingDay(date))
                throw new Exception("Date is already a non trading day");

            var @event = new NonTradingDayAddedEvent(date);
            ApplyEvent(@event);

            _EventStore.StoreEvent(Guid.Empty, @event);
        }

        public void ApplyEvent(NonTradingDayAddedEvent e)
        {
            _TradingCalander.AddNonTradingDay(e.Date);
        }

        public void UpdateClosingPrice(string asxCode, DateTime date, decimal closingPrice)
        {
            // Check that stock exists
            var stock = GetStock(asxCode, date);
            if (stock == null)
                throw new Exception("Stock not found");

            if (!_TradingCalander.IsTradingDay(date))
                throw new Exception("Date is not a trading day");
             
            var @event = new ClosingPriceAddedEvent(stock.Id, date, closingPrice);
            ApplyEvent(@event);

            _EventStore.StoreEvent(Guid.Empty, @event);
        }

        public void ApplyEvent(ClosingPriceAddedEvent e)
        {
            var stock = GetStock(e.Id);

            stock.AddPrice(e.Date, e.ClosingPrice);
        }

        public void UpdateCurrentPrice(string asxCode, decimal currentPrice)
        {
            // Check that stock exists
            var stock = GetStock(asxCode, DateTime.Now);
            if (stock == null)
                throw new Exception("Stock not found");

            // Update the current price directly here as this event is not stored
            stock.AddPrice(DateTime.Now, currentPrice);
        }
    }
}
