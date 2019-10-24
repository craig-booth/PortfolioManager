using System;
using System.Collections.Generic;
using System.Linq;
using Booth.Common;

using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{

    public struct StockPrice
    {
        public DateTime Date;
        public decimal Price;

        public StockPrice(DateTime date, decimal price)
        {
            Date = date;
            Price = price;
        }
    }

    public interface IStockPriceHistory
    {
        Guid Id { get; }
        DateTime EarliestDate { get; }
        DateTime LatestDate { get; }
        decimal GetPrice(DateTime date);
        IEnumerable<StockPrice> GetPrices(DateRange dateRange);
    }

    public class StockPriceHistory : TrackedEntity, IStockPriceHistory
    {
        private SortedList<DateTime, decimal> _Prices { get; } = new SortedList<DateTime, decimal>();

        public StockPriceHistory(Guid id)
            : base(id)
        {

        }

        public DateTime EarliestDate
        {
            get
            {
                if (_Prices.Count > 0)
                    return _Prices.First().Key;
                else
                    return DateUtils.NoDate;
            }
        }

        public DateTime LatestDate
        {
            get
            {
                if (_Prices.Count > 0)
                    return _Prices.Last().Key;
                else
                    return DateUtils.NoDate;
            }
        }

        public decimal GetPrice(DateTime date)
        {
            var index = IndexOf(date);

            if (index >= 0)
                return _Prices.Values[index];
            else if (index == -1)
                return 0.00m;
            else
                return _Prices.Values[~index];

        }

        public IEnumerable<StockPrice> GetPrices(DateRange dateRange)
        {
            var first = IndexOf(dateRange.FromDate);
            if (first == -1)
                first = 0;
            else if (first < 0)
                first = ~first;

            var last = IndexOf(dateRange.ToDate);
            if (last == -1)
                last = 0;
            else if (last < 0)
                last = ~last;

            return _Prices.Skip(first).Take(last - first + 1).Select(x => new StockPrice(x.Key, x.Value));
        }

        public void UpdateCurrentPrice(decimal currentPrice)
        {
            UpdatePrice(DateTime.Today, currentPrice);
        }

        public void UpdateClosingPrice(DateTime date, decimal closingPrice)
        {
            var @event = new ClosingPricesAddedEvent(Id, Version, new ClosingPricesAddedEvent.ClosingPrice[] { new ClosingPricesAddedEvent.ClosingPrice(date, closingPrice) });
            Apply(@event);

            PublishEvent(@event);
        }

        public void UpdateClosingPrices(IEnumerable<StockPrice> closingPrices)
        {
            var @event = new ClosingPricesAddedEvent(Id, Version, closingPrices.Select(x => new ClosingPricesAddedEvent.ClosingPrice(x.Date, x.Price)));
            Apply(@event);

            PublishEvent(@event);
        }

        public void Apply(ClosingPricesAddedEvent @event)
        {
            Version++;
            foreach (var closingPrice in @event.ClosingPrices)
                UpdatePrice(closingPrice.Date, closingPrice.Price);
        }

        private void UpdatePrice(DateTime date, decimal price)
        {
            if (_Prices.ContainsKey(date))
                _Prices[date] = price;
            else
                _Prices.Add(date, price);
        }

        private int IndexOf(DateTime date)
        {
            if (_Prices.Keys.Count == 0)
                return -1;

            int begin = 0;
            int end = _Prices.Keys.Count;
            while (end > begin)
            {
                int index = (begin + end) / 2;
                var el = _Prices.Keys[index];

                var r = el.CompareTo(date);
                if (r == 0)
                    return index;
                else if (r > 0)
                    end = index;
                else
                    begin = index + 1;
            }

            if (end == 0)
                return -1;
            else
                return -end;
        }
    }
}
