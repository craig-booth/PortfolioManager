using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Utilities
{
    public enum ParameterType { SingleDate, DateRange, FinancialYear }
    public class ReportParmeter
    {
        public ParameterType Type { get; private set; }

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        private ReportParmeter(ParameterType type, DateTime fromDate, DateTime toDate)
        {
            Type = type;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public override string ToString()
        {
            return String.Format("From {0:d} To {1:d}", FromDate, ToDate);
        }

        // Single Date
        public static ReportParmeter Date(DateTime date)
        {
            return new ReportParmeter(ParameterType.SingleDate, date, date);
        }

        public static ReportParmeter Today()
        {
            return new ReportParmeter(ParameterType.SingleDate, DateTime.Today, DateTime.Today);
        }

        // Date Range
        public static ReportParmeter DateRange(DateTime fromDate, DateTime toDate)
        {
            return new ReportParmeter(ParameterType.DateRange, fromDate, toDate);
        }

        // Financial Year
        public static ReportParmeter FinancialYear(int year)
        {
            return new ReportParmeter(ParameterType.FinancialYear, new DateTime(year - 1, 7, 1), new DateTime(year, 6, 30));
        }

        public static ReportParmeter CurrentFinancialYear()
        {
            if (DateTime.Today.Month <= 6)
                return FinancialYear(DateTime.Today.Year);
            else
                return FinancialYear(DateTime.Today.Year + 1);
        }


        public static ReportParmeter PreviousFinancialYear()
        {
            if (DateTime.Today.Month > 6)
                return FinancialYear(DateTime.Today.Year);
            else
                return FinancialYear(DateTime.Today.Year - 1);
        }
    }
}
