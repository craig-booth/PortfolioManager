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

        public static bool operator ==(DateRange a, DateRange b)
        {
            return (a.FromDate == b.FromDate) && (a.ToDate == b.ToDate);
        }

        public static bool operator !=(DateRange a, DateRange b)
        {
            return (a.FromDate != b.FromDate) || (a.ToDate != b.ToDate);
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", FromDate, ToDate);
        }

        public override bool Equals(object obj)
        {
            if (obj is DateRange)
                return this == (DateRange)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (FromDate.GetHashCode() * -1521134295) + ToDate.GetHashCode();
        }
    }
}