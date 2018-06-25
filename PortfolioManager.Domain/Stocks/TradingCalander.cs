using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.EventStore;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Domain.Stocks
{
    public interface ITradingCalander
    {
        IEnumerable<DateTime> NonTradingDays();
        bool IsTradingDay(DateTime date);
        IEnumerable<DateTime> TradingDays(DateRange range);
    }

    public class TradingCalander : ITradingCalander
    {
        public static readonly Guid Id = new Guid("712E464B-1CE6-4B21-8FB2-D679DFFE3EE3");

        public int Version { get; private set; } = 0;
        private IEventStream _EventStream;

        private List<DateTime> _NonTradingDays = new List<DateTime>();

        public TradingCalander(IEventStream eventStream)
        {
            _EventStream = eventStream;
        }

        public void LoadFromEventStream()
        {
            var events = _EventStream.RetrieveEvents();
            foreach (var @event in events)
            {
                dynamic dynamicEvent = @event;
                Apply(dynamicEvent);
            }
        }

        public void AddNonTradingDay(DateTime date)
        {
            // Check that the non trading day is not already defined
            if (!IsTradingDay(date))
                throw new Exception("Date is already a non trading day");

            var @event = new NonTradingDayAddedEvent(Id, Version, date);
            Apply(@event);

            _EventStream.StoreEvent(@event);
        }

        public void Apply(NonTradingDayAddedEvent @event)
        {
            Version++;
            var index = _NonTradingDays.BinarySearch(@event.Date);
            if (index < 0)
            {
                _NonTradingDays.Insert(~index, @event.Date);
            }
        }

        public IEnumerable<DateTime> NonTradingDays()
        {
            return _NonTradingDays;
        }

        public bool IsTradingDay(DateTime date)
        {
            return (_NonTradingDays.BinarySearch(date) < 0);
        }

        public IEnumerable<DateTime> TradingDays(DateRange range)
        {
            return DateUtils.DateRange(range.FromDate, range.ToDate).Where(x => x.WeekDay() && IsTradingDay(x));
        }
    }
}
