using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Model.Utils
{
    public static class DateUtils
    {
        private readonly static DateTime _NoStartDate = new DateTime(0001, 01, 01);
        private readonly static DateTime _NoEndDate = new DateTime(9999, 12, 31);

        public static DateTime NoDate
        {
            get { return _NoStartDate; }
        }

        public static DateTime NoStartDate 
        {
            get { return _NoStartDate; }
        }

        public static DateTime NoEndDate
        {
            get { return _NoEndDate; }
        }

        public static int FinancialYear(this DateTime date)
        {
            if (date.Month <= 6)
                return date.Year;
            else
                return  date.Year + 1;
        }

        public static DateTime StartOfFinancialYear(int financialYear)
        {
            return  new DateTime(financialYear - 1, 7, 1);
        }

        public static DateTime EndOfFinancialYear(int financialYear)
        {
            return new DateTime(financialYear, 6, 30);
        }

        public static Boolean WeekDay(this DateTime date)
        {
            return (date.DayOfWeek >= DayOfWeek.Monday) && (date.DayOfWeek <= DayOfWeek.Friday);
        }

        public static IEnumerable<DateTime> DateRange(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, toDate.Subtract(fromDate).Days + 1)
                             .Select(d => fromDate.AddDays(d));
        }

        public static DateTime Earlist(DateTime date1, DateTime date2)
        {
            return  date1 <= date2 ? date1 : date2;
        }

        public static DateTime Latest(DateTime date1, DateTime date2)
        {
            return date1 >= date2 ? date1 : date2;
        }

    }
}
