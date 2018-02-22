using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;

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
        private List<DateTime> _NonTradingDays = new List<DateTime>();

        public void AddNonTradingDay(DateTime date)
        {
            var index = _NonTradingDays.BinarySearch(date);
            if (index < 0)
            {
                _NonTradingDays.Insert(~index, date);
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
