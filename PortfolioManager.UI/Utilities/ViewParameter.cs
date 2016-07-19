using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using PortfolioManager.Model.Stocks;

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

    interface IStockParameter: INotifyPropertyChanged
    {
        Stock Stock { get; }
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
        private DateTime _StartDate;
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

        private DateTime _EndDate;
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

        private DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
        }

        public FinancialYearParameter()
            : base()
        {
            // Determine current financial year
            if (DateTime.Today.Month <= 6)
                _FinancialYear = DateTime.Today.Year;
            else
                _FinancialYear = DateTime.Today.Year + 1;
        }
    }

    class StockParameter : NotifyClass, IStockParameter
    {
        private Stock _Stock;
        public Stock Stock
        {
            get
            {
                return _Stock;
            }

            set
            {
                _Stock = value;
                OnPropertyChanged();
            }
        }
    }
}
