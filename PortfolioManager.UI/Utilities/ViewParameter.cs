using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PortfolioManager.UI.Utilities
{
    interface ISingleDateParameter : INotifyPropertyChanged
    {
        DateTime Date { get; }
    }

    interface IDateRangeParameter : INotifyPropertyChanged
    {
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }

    interface IFinancialYearParameter : IDateRangeParameter
    {
        int FinancialYear { get; }
    }

    class SingleDateParameter : NotifyClass, ISingleDateParameter
    {
        protected DateTime _Date;
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


        public SingleDateParameter()
        {
            _Date = DateTime.Now;
        }
    }

    class DateRangeParameter : NotifyClass, IDateRangeParameter
    {
        protected DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }

            set
            {
                _StartDate = value;
                OnPropertyChanged();
            }
        }

        protected DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }

            set
            {
                _EndDate = value;
                OnPropertyChanged();
            }
        }

        public DateRangeParameter()
        {
            _StartDate = DateTime.Now.AddYears(-1).AddDays(1);
            _EndDate = DateTime.Now;
        }
    }

    class FinancialYearParameter : NotifyClass, IFinancialYearParameter
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
                _StartDate = new DateTime(_FinancialYear - 1, 7, 1);
                _EndDate = new DateTime(_FinancialYear, 6, 30);
                OnPropertyChanged("");
            }
        }

        protected DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
        }

        protected DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
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
            : base()
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
                    _FinancialYears.Add(year, "Current");
                else if (year == currentFinancialYear - 1)
                    _FinancialYears.Add(year, "Previous");
                else
                    _FinancialYears.Add(year, string.Format("{0} - {1}", year - 1, year));

                year--;
            }

            _FinancialYear = currentFinancialYear;
        }
    }
}
