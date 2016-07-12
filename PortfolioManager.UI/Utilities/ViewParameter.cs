using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.UI.Utilities
{
    interface ISingleDateParameter
    {
        DateTime Date { get; }
    }

    interface IDateRangeParameter
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }

    interface IFinancialYearParameter : IDateRangeParameter
    {
        int FinancialYear { get; }
    }

    class ViewParameter : NotifyClass
    {

    }

    class FinancialYearParameter : ViewParameter, IFinancialYearParameter
    {
        private int _FinancialYear;
        public int FinancialYear
        {
            get
            {
                return _FinancialYear;
            }

            set
            {
                _FinancialYear = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartDate
        {
            get
            {
                return new DateTime(_FinancialYear - 1, 7, 1);
            }
        }

        public DateTime EndDate
        {
            get
            {
                return new DateTime(_FinancialYear, 6, 30);
            }
        }

        public string Description
        {
            get
            {
                return string.Format("{0}/{1} Financial Year", _FinancialYear - 1, _FinancialYear);
            }
        }

        private Dictionary<int, string> _FinancialYears;
        public IReadOnlyDictionary<int, string> FinancialYears
        {
            get
            {
                return _FinancialYears;
            }
        }

        public FinancialYearParameter(int oldestYear)
            : this(oldestYear, 0)
        {

        }

        public FinancialYearParameter(int oldestYear, int selectedYear)
        {
            _FinancialYears = new Dictionary<int, string>();

            // Determine current financial year
            int currentFinancialYear;
            if (DateTime.Today.Month <= 6)
                currentFinancialYear = DateTime.Today.Year;
            else
                currentFinancialYear = DateTime.Today.Year + 1;

            // populate list of financial years
            int year = currentFinancialYear;
            while (year >= oldestYear)
            {
                if (year == currentFinancialYear)
                    _FinancialYears.Add(year, "Current Financial Year");
                else if (year == currentFinancialYear - 1)
                    _FinancialYears.Add(year, "Previous Financial Year");
                else
                    _FinancialYears.Add(year, string.Format("{0} - {1}", year - 1, year));

                year--;
            }

            // set the parameter value
            if (selectedYear > 0)
                _FinancialYear = selectedYear;
            else
                _FinancialYear = currentFinancialYear;
        }
    }

    class SingleDateParameter : ViewParameter, ISingleDateParameter
    {
        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }

            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return string.Format("{0:d}", Date);
            }
        }

        public SingleDateParameter()
            : this(DateTime.Now)
        {

        }

        public SingleDateParameter(DateTime date)
        {
            Date = DateTime.Now;
        }
    }

    class DateRangeParameter : ViewParameter, IDateRangeParameter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public string Description
        {
            get
            {
                return string.Format("{0:d} - {1:d}", StartDate, EndDate);
            }
        }

        public DateRangeParameter()
        {
            StartDate = DateTime.Now.AddYears(-1);
            EndDate = DateTime.Now;
        }

        public DateRangeParameter(DateTime start, DateTime end)
        {
            StartDate = start;
            EndDate = end;
        }
    }
}
