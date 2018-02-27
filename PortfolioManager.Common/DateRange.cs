using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioManager.Common
{
    public struct DateRange
    {
        public DateTime FromDate;
        public DateTime ToDate;

        public DateRange(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        public bool Contains(DateTime date)
        {
            return (FromDate <= date) && (ToDate >= date);
        }

        public bool Overlaps(DateRange dateRange)
        {
            return (FromDate <= dateRange.FromDate && ToDate >= dateRange.FromDate) ||
                   (FromDate > dateRange.FromDate && FromDate <= dateRange.ToDate);
        }
    }
}
