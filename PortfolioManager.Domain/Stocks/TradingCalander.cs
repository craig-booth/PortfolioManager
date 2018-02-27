using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
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
        private IEventStore _EventStore;
        private List<DateTime> _NonTradingDays = new List<DateTime>();

        public TradingCalander(IEventStore eventStore)
        {
            _EventStore = eventStore;
        }

        public void AddNonTradingDay(DateTime date)
        {
            // Check that the non trading day is not already defined
            if (!IsTradingDay(date))
                throw new Exception("Date is already a non trading day");

            var @event = new NonTradingDayAddedEvent(date);
            Apply(@event);

            _EventStore.StoreEvent(Guid.Empty, @event);
        }

        public void Apply(NonTradingDayAddedEvent e)
        {
            var index = _NonTradingDays.BinarySearch(e.Date);
            if (index < 0)
            {
                _NonTradingDays.Insert(~index, e.Date);
            }
        }

        public IEnumerable<DateTime> NonTradingDays()
        {
            return _NonTradingDays;
        }

        public bool IsTradingDay(DateTime date)
        {
            return (_NonTradingDays.BinarySearch(date) >= 0);
        }

        public IEnumerable<DateTime> TradingDays(DateRange range)
        {
            return DateUtils.DateRange(range.FromDate, range.ToDate).Where(x => x.WeekDay() && IsTradingDay(x));
        }
    }
}
