using System;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.Service.Interface;

namespace PortfolioManager.UI.Utilities
{
    class ViewParameter : NotifyClass
    {
        public RestWebClient RestWebClient;
        public RestClient RestClient;

        private StockItem _Stock;
        public StockItem Stock
        {
            get
            {
                return _Stock;
            }

            set
            {
                if (_Stock != value)
                {
                    _Stock = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }

            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateRange _DateRange;
        public DateRange DateRange
        {
            get
            {
                return _DateRange;
            }

            set
            {
                if (_DateRange != value)
                {
                    DateRange = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime FromDate
        {
            get
            {
                return _DateRange.FromDate;
            }

            set
            {
                if (_DateRange.FromDate != value)
                {
                    _DateRange = new DateRange(value, _DateRange.ToDate);
                    OnPropertyChanged("DateRange");
                }
            }
        }

        public DateTime ToDate
        {
            get
            {
                return _DateRange.ToDate;
            }

            set
            {
                if (_DateRange.ToDate != value)
                {
                    _DateRange = new DateRange(_DateRange.FromDate, value);
                    OnPropertyChanged("DateRange");
                }
            }
        }


        private int _FinancialYear;
        public int FinancialYear
        {
            get
            {
                return _FinancialYear;
            }

            set
            {
                if (_FinancialYear != value)
                {
                    _FinancialYear = value;
                    OnPropertyChanged();
                }
            }
        }

        public ViewParameter()
        {
            _Date = DateTime.Now;
            _DateRange = new DateRange(DateTime.Now.AddYears(-1).AddDays(1), DateTime.Now);
            _FinancialYear = DateTime.Today.FinancialYear();
        }

    }
}
